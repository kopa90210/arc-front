using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Myshop.Core.Models;
using Myshop.Core.Dtos;
using Myshop.Core.Interfaces;
using Myshop.Application.Interfaces;
using Myshop.Application.Emails;
using Myshop.Application.Emails.Dtos;
using Myshop.Application.Dtos;

namespace Myshop.Application.Services
{
    public class OrderService : IOrderService
    {
        // private readonly AppDbContext _db;

         private readonly IOrderRepo _orderRepo;
         private readonly IProductRepo _productRepo;
         private readonly IVendorRepo _vendorRepo;
         private readonly IEmailService _email;  
         private readonly IOrderNotifier _notifier;  

        public OrderService(IOrderRepo orderRepo, IEmailService email ,IVendorRepo  vendorRepo, IOrderNotifier notifier, IProductRepo productRepo) 
        { 
            _orderRepo = orderRepo;
            _email = email;
            _notifier = notifier; 
            _vendorRepo = vendorRepo;
            _productRepo = productRepo;
        }

        // public OrderService(AppDbContext db)
        // {
        //     _db = db;
        // }

        // public async Task<List<Order>> GetMyOrdersAsync(string vendorId)
        // {
        //     var orders = await _orderRepo.Orders
        //         .Include(o => o.Items)
        //         .ThenInclude(oi => oi.Product)
        //         .Include(o => o.Payment)
        //         .Include(o => o.Shipping)
        //         .Where(o => o.Items.Any(oi => oi.Product != null && oi.Product.VendorId == vendorId))
        //         .OrderByDescending(o => o.CreatedAt)
        //         .ToListAsync();

        //     System.Console.WriteLine($"OrderService.GetMyOrdersAsync({vendorId}): Found {orders.Count} orders");
        //     return orders;
        // }
     // ── 1. Checkout ───────────────────────────────────────────────────────
        // Maps to POST /api/orders/checkout
        public async Task<OrderResultDto> CheckoutAsync(
            CheckoutOrderDto request,
            ClaimsPrincipal  user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new UnauthorizedAccessException("No user ID in token.");
 
            var customerEmail = user.FindFirstValue(ClaimTypes.Email)
                             ?? user.FindFirstValue("email") ?? "";
 
            var customerName  = user.FindFirstValue(ClaimTypes.Name)
                             ?? user.FindFirstValue("name")  ?? "Customer";
 
            var order = new Order
            {
                UserId        = userId,
                CustomerId    = request.CustomerId,
                Currency      = string.IsNullOrWhiteSpace(request.Currency) ? "EGP" : request.Currency,
                CustomerEmail = customerEmail,
                CustomerName  = customerName,
                Shipping = new OrderShipping
                {
                    RecipientName = request.Shipping?.RecipientName,
                    Phone         = request.Shipping?.Phone,
                    AddressLine1  = request.Shipping?.AddressLine1,
                    AddressLine2  = request.Shipping?.AddressLine2,
                    City          = request.Shipping?.City,
                    State         = request.Shipping?.State,
                    PostalCode    = request.Shipping?.PostalCode,
                    Country       = request.Shipping?.Country
                },
                Payment = new OrderPayment
                {
                    Provider = request.PaymentMethod ?? "cash",
                    Status   = "Pending",
                    Currency = string.IsNullOrWhiteSpace(request.Currency) ? "EGP" : request.Currency
                }
            };
 
            // Resolve products — freezing name and price at order time
            foreach (var req in request.Items)
            {
                var product = await _productRepo.Products
                    .Where(p => p != null && p.Id == req.ProductId)
                    .FirstOrDefaultAsync()
                    ?? throw new KeyNotFoundException($"Product {req.ProductId} not found.");
 
                order.Items.Add(new OrderItem
                {
                    ProductId       = product.Id,
                    ProductName     = product.Name,
                    PriceAtPurchase = product.Price,
                    Quantity        = req.Quantity,
                    VendorId        = product.VendorId
                });
 
                order.Total += product.Price * req.Quantity;
            }
 
            order.Payment.Amount = order.Total;
            order.TransitionTo(OrderStatus.PendingPayment);
 
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveAsync();
 
            return order.ToResultDto("Checkout created. Awaiting payment.");
        }
 



       // ── 2. Payment success ────────────────────────────────────────────────
        // Maps to POST /api/orders/{id}/payment-success
        // Wrapped in a DB transaction — stock reduction must be atomic.
        public async Task<OrderResultDto> PaymentSuccessAsync(
            int              orderId,
            PaymentSuccessDto dto,
            ClaimsPrincipal  user)
        {
            OrderResultDto result = null!;
 
            await _orderRepo.ExecuteInTransactionAsync(async () =>
            {
                var order = await _orderRepo.GetByIdAsync(orderId)
                            ?? throw new KeyNotFoundException($"Order {orderId} not found.");
 
                if (order.Status != OrderStatus.PendingPayment)
                    throw new InvalidOperationException(
                        $"Cannot confirm payment — current status: {order.Status}");
 
                // ── Validate stock for ALL items before touching anything ────
                foreach (var item in order.Items)
                {
                    if (item.Product == null)
                        throw new InvalidOperationException($"Product {item.ProductId} not found.");
 
                    if (item.Product.Stock < item.Quantity)
                    {
                        // Insufficient stock: auto-refund the order
                        order.TransitionTo(OrderStatus.Refunded);
                        RecordPayment(order, dto.Provider, dto.TransactionId, "Refunded");
                        await _orderRepo.SaveAsync();
                        result = order.ToResultDto("Insufficient stock. Order refunded.");
                        return;
                    }
                }


                // ── Reduce stock ─────────────────────────────────────────────
                foreach (var item in order.Items)
                    item.Product!.Stock -= item.Quantity;
 
                order.TransitionTo(OrderStatus.Paid);
                RecordPayment(order, dto.Provider, dto.TransactionId, "Paid");
                await _orderRepo.SaveAsync();
 
                // ── Credit vendor balances ────────────────────────────────────
                var vendorGroups = order.Items
                    .Where(i => i.VendorId != null)
                    .GroupBy(i => i.VendorId!);
 
                foreach (var group in vendorGroups)
                {
                    var earned = group.Sum(i => i.PriceAtPurchase * i.Quantity);
                    await _vendorRepo.UpdateBalanceAsync(group.Key, earned);
                }

//                 foreach (var item in order.Items)
// {
//     var product = await _productRepo.Products.FindAsync(item.ProductId);
//     if (product != null)
//     {
//         product.Stock -= item.Quantity;
//           // <-- increment sales counter
//     }
// }
// await _productRepo.SaveChangesAsync();

 

    // ── Real-time + email notifications ───────────────────────────
                // Fire-and-forget: don't let a notification failure roll back the payment
                _ = Task.Run(() => SendPostPaymentNotificationsAsync(order));
 
                result = order.ToResultDto("Payment confirmed. Stock reduced.");
            });
 
            return result;

            
        }





         // ── 3. Payment failed ─────────────────────────────────────────────────
        // Maps to POST /api/orders/{id}/payment-failed
        public async Task<OrderResultDto> PaymentFailedAsync(
            int             orderId,
            PaymentFailedDto dto,
            ClaimsPrincipal  user)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                        ?? throw new KeyNotFoundException($"Order {orderId} not found.");
 
            if (order.Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException(
                    $"Cannot cancel — current status: {order.Status}");
 
            order.TransitionTo(OrderStatus.Cancelled);
            RecordPayment(order, dto.Provider, dto.TransactionId, "Failed");
 
            await _orderRepo.SaveAsync();
            return order.ToResultDto("Payment failed. Order cancelled.");
        }



         // ── 4. Get customer's own orders ──────────────────────────────────────
        public async Task<List<OrderSummaryDto>> GetMyOrdersAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new UnauthorizedAccessException();
 
            return await _orderRepo.Query
                .Include(o => o.Items)
                .Include(o => o.Shipping)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.ToSummaryDto())
                .ToListAsync();
        }
 
        // ── 5. Get vendor's orders ────────────────────────────────────────────
        public async Task<List<OrderSummaryDto>> GetVendorOrdersAsync(string vendorId)
        {
            return await _orderRepo.Query
                .Include(o => o.Items)
                .Include(o => o.Shipping)
                .Where(o => o.Items.Any(i => i.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.ToSummaryDto())
                .ToListAsync();
        }
 

  // ── 6. Vendor processes the order ─────────────────────────────────────────
        public async Task<OrderResultDto> ProcessOrderAsync(int orderId, ClaimsPrincipal vendor)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                        ?? throw new KeyNotFoundException($"Order {orderId} not found.");
            
            if (order.Status != OrderStatus.Paid)
                throw new InvalidOperationException($"Order must be Paid to start processing. Current: {order.Status}");
            
            order.TransitionTo(OrderStatus.Processing);
            await _orderRepo.SaveAsync();

            return order.ToResultDto("Order is now processing.");
        }

  // ── 7. Vendor ships the order ─────────────────────────────────────────
        public async Task<OrderResultDto> ShipOrderAsync(int orderId, ShipOrderDto dto, ClaimsPrincipal vendor)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)     
                        ?? throw new KeyNotFoundException($"Order {orderId} not found.");
 
            if (order.Status != OrderStatus.Paid && order.Status != OrderStatus.Processing)
                throw new InvalidOperationException(
                    $"Cannot ship order with status {order.Status}");
 
            order.TransitionTo(OrderStatus.Shipped);
 
            if (order.Shipping != null)
            {
                order.Shipping.TrackingNumber = dto.TrackingNumber;
                order.Shipping.Carrier        = dto.Carrier;
                order.Shipping.ShippedAt      = DateTime.UtcNow;
            }
 
            await _orderRepo.SaveAsync();
 
            // Email customer — email is stored on the order so no extra query needed
            if (!string.IsNullOrEmpty(order.CustomerEmail))
            {
                _ = Task.Run(() => _email.SendShippingUpdateAsync(new ShippingUpdateEmailDto(
                    CustomerEmail:     order.CustomerEmail,
                    CustomerName:      order.CustomerName ?? order.Shipping?.RecipientName ?? "Customer",
                    OrderId:           order.Id,
                    TrackingNumber:    dto.TrackingNumber,
                    Carrier:           dto.Carrier,
                    EstimatedDelivery: dto.EstimatedDelivery
                )));
            }
 
            return order.ToResultDto("Order shipped.");
        }

  // ── 8. Vendor delivers the order ─────────────────────────────────────────
        public async Task<OrderResultDto> DeliverOrderAsync(int orderId, ClaimsPrincipal vendor)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                        ?? throw new KeyNotFoundException($"Order {orderId} not found.");
            
            if (order.Status != OrderStatus.Shipped)
                throw new InvalidOperationException($"Order must be Shipped to deliver. Current: {order.Status}");
            
            if (order.Shipping == null)
                order.Shipping = new OrderShipping();
            
            order.Shipping.DeliveredAt = DateTime.UtcNow;
            order.TransitionTo(OrderStatus.Delivered);
            await _orderRepo.SaveAsync();

            return order.ToResultDto("Order delivered.");
        }
 
        // ── 9. Admin: all orders ──────────────────────────────────────────────
        public async Task<List<OrderSummaryDto>> GetAllOrdersAsync()
        {
            return await _orderRepo.Query
                .Include(o => o.Items)
                .Include(o => o.AppUser)
                .Include(o => o.Shipping)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.ToSummaryDto())
                .ToListAsync();
        }




        // ── Private helpers ───────────────────────────────────────────────────
 
        private static void RecordPayment(
            Order   order,
            string? provider,
            string? transactionId,
            string  status)
        {
            if (order.Payment == null)
                order.Payment = new OrderPayment { Currency = order.Currency };
 
            if (!string.IsNullOrWhiteSpace(provider))
                order.Payment.Provider = provider;
 
            order.Payment.TransactionId = transactionId;
            order.Payment.Amount        = order.Total;
            order.Payment.Status        = status;
 
            if (status is "Paid" or "Refunded")
                order.Payment.PaidAt = DateTime.UtcNow;
        }
 
        private async Task SendPostPaymentNotificationsAsync(Order order)
        {
            try
            {
                // Customer confirmation email
                if (!string.IsNullOrEmpty(order.CustomerEmail))
                {
                    await _email.SendOrderConfirmationAsync(new OrderConfirmationEmailDto(
                        CustomerEmail:   order.CustomerEmail,
                        CustomerName:    order.CustomerName ?? "Customer",
                        OrderId:         order.Id,
                        Total:           order.Total,
                        Currency:        order.Currency,
                        PaymentMethod:   order.Payment?.Provider ?? "—",
                        Items:           order.Items
                                              .Select(i => new OrderItemLine(
                                                  i.ProductName, i.Quantity, i.PriceAtPurchase))
                                              .ToList(),
                        ShippingAddress: FormatAddress(order.Shipping)
                    ));
                }
 
                // Per-vendor: real-time SignalR + email
                var vendorGroups = order.Items
                    .Where(i => i.VendorId != null)
                    .GroupBy(i => i.VendorId!);
 
                foreach (var group in vendorGroups)
                {
                    var vendorId = group.Key;
                    var notification = new OrderNotificationDto
                    {
                        OrderId   = order.Id,
                        Total     = group.Sum(i => i.PriceAtPurchase * i.Quantity),
                        ItemCount = group.Count(),
                        Status    = order.Status.ToString(),
                        CreatedAt = DateTime.UtcNow
                    };
 
                    await _notifier.NotifyVendorNewOrder(vendorId, notification);
 
                    var vendor = await _vendorRepo.GetByIdAsync(vendorId);
                    if (vendor?.Email is null) continue;
 
                    await _email.SendVendorOrderNotificationAsync(new VendorOrderEmailDto(
                        VendorEmail:  vendor.Email,
                        VendorName:   vendor.StoreName ?? vendor.Email,
                        OrderId:      order.Id,
                        CustomerName: order.CustomerName ?? "Customer",
                        Total:        notification.Total,
                        Currency:     order.Currency,
                        Items:        group.Select(i => new OrderItemLine(
                                          i.ProductName, i.Quantity, i.PriceAtPurchase)).ToList(),
                        PlacedAt:     order.CreatedAt
                    ));
                }
            }
            catch (Exception ex)
            {
                // Log but never throw — notifications must never roll back a payment
                Console.Error.WriteLine($"[OrderService] Notification error on order {order.Id}: {ex.Message}");
                // Replace with ILogger<OrderService> in production
            }
        }
 
        private static string FormatAddress(OrderShipping? s)
        {
            if (s is null) return "—";
            return $"{s.AddressLine1}{(s.AddressLine2 != null ? ", " + s.AddressLine2 : "")}, " +
                   $"{s.City}, {s.State} {s.PostalCode}, {s.Country}";
        }

        public async Task<List<Order>> GetMyOrdersAsync(string vendorId)
        {
            return await _orderRepo.Query
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .Where(o => o.Items.Any(i => i.Product != null && i.Product.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepo.Query
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }


        // GetVendorOrdersAsync overload with limit — satisfies IOrderService contract
        public async Task<List<OrderSummaryDto>> GetVendorOrdersAsync(
            string vendorId, int limit = 5)
        {
            return await _orderRepo.Query
                .Include(o => o.Items)
                .Include(o => o.Shipping)
                .Where(o => o.Items.Any(i => i.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .Select(o => o.ToSummaryDto())
                .ToListAsync();
        }



        // ── NEW: 7-day revenue breakdown for chart ────────────────────────────
        public async Task<List<RevenueByDayDto>> GetRevenueChartAsync(
            string vendorId, string range)
        {
            var from = range switch
            {
                "Today"      => DateTime.UtcNow.Date,
                "This Month" => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                "This Year"  => new DateTime(DateTime.UtcNow.Year, 1, 1),
                _            => DateTime.UtcNow.AddDays(-6).Date, // "This Week" default
            };

            var rows = await _orderRepo.Query
                .Where(o =>
                    o.CreatedAt >= from &&
                    o.Items.Any(oi => oi.Product != null && oi.Product.VendorId == vendorId) &&
                    (o.Status == OrderStatus.Paid ||
                     o.Status == OrderStatus.Processing ||
                     o.Status == OrderStatus.Shipped ||
                     o.Status == OrderStatus.Delivered))
                .GroupBy(o => o.CreatedAt.DayOfWeek)
                .Select(g => new RevenueByDayDto
                {
                    DayOfWeek = (int)g.Key,
                    Revenue   = g.SelectMany(o => o.Items)
                                 .Where(i => i.VendorId == vendorId)
                                 .Sum(i => (decimal?)i.PriceAtPurchase * i.Quantity) ?? 0m,
                })
                .ToListAsync();

            // Map to Mon–Sun labels
            var days = new[] { "Mon","Tue","Wed","Thu","Fri","Sat","Sun" };
            return days.Select((d, i) =>
            {
                int dow = i + 1; // Mon=1…Sat=6, Sun mapped to 0
                if (i == 6) dow = 0;
                var match = rows.FirstOrDefault(r => r.DayOfWeek == dow);
                return new RevenueByDayDto { Day = d, Revenue = match?.Revenue ?? 0m };
            }).ToList();
        }

        // ── NEW: top-selling products by units ──────────────────────────────
        public async Task<List<TopProductDto>> GetTopSellingAsync(
            string vendorId, int top = 3)
        {
            var results = await _orderRepo.Query
                .Where(o => o.Status >= OrderStatus.Paid)
                .SelectMany(o => o.Items)
                .Where(i => i.VendorId == vendorId)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    Name      = g.Key.ProductName,
                    Units     = g.Sum(i => i.Quantity),
                })
                .OrderByDescending(x => x.Units)
                .Take(top)
                .ToListAsync();

            for (int i = 0; i < results.Count; i++)
                results[i].Rank = i + 1;

            return results;
        }
    }
 
    // ── Projection extensions ─────────────────────────────────────────────────
    // Keeps mapping logic out of both the controller and the service body.
 
    internal static class OrderMappingExtensions
    {
        public static OrderResultDto ToResultDto(this Order o, string message) => new()
        {
            Id       = o.Id,
            Status   = o.Status.ToString(),
            Total    = o.Total,
            Currency = o.Currency,
            Message  = message
        };
 
        public static OrderSummaryDto ToSummaryDto(this Order o) => new()
        {
            Id             = o.Id,
            Status         = o.Status.ToString(),
            Total          = o.Total,
            Currency       = o.Currency,
            CreatedAt      = o.CreatedAt,
            ItemCount      = o.Items?.Count ?? 0,
            CustomerEmail  = o.CustomerEmail,
            TrackingNumber = o.Shipping?.TrackingNumber
        };
    }

  
    


}
    

