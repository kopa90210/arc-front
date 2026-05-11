
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

// namespace Myshop.Core.Models
// {
//     public class Order
//     {
//         public int Id { get; set; }

//         [ForeignKey("AppUser")]
//         public string UserId { get; set; } = string.Empty;
//         public AppUser? AppUser { get; set; }

//         public int? CustomerId { get; set; }
//         public Customer? Customer { get; set; }

//         public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//         public decimal Total { get; set; }

//         public OrderStatus Status { get; private set; } = OrderStatus.Draft;

//         public OrderPayment? Payment { get; set; }
//         public OrderShipping? Shipping { get; set; }

//         public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

//         public void TransitionTo(OrderStatus nextStatus)
//         {
//             if (!IsValidTransition(Status, nextStatus))
//             {
//                 throw new InvalidOperationException($"Invalid status transition: {Status} -> {nextStatus}");
//             }

//             Status = nextStatus;
//         }

//         private static bool IsValidTransition(OrderStatus current, OrderStatus next)
//         {
//             return current switch
//             {
//                 OrderStatus.Draft => next is OrderStatus.PendingPayment or OrderStatus.Cancelled,
//                 OrderStatus.PendingPayment => next is OrderStatus.Paid or OrderStatus.Cancelled,
//                 OrderStatus.Paid => next is OrderStatus.Processing or OrderStatus.Refunded,
//                 OrderStatus.Processing => next is OrderStatus.Shipped or OrderStatus.Cancelled or OrderStatus.Refunded,
//                 OrderStatus.Shipped => next is OrderStatus.Delivered or OrderStatus.Refunded,
//                 OrderStatus.Delivered => next is OrderStatus.Refunded,
//                 OrderStatus.Cancelled => false,
//                 OrderStatus.Refunded => false,
//                 _ => false
                
//             };
//         }
//     }
    
//     public class OrderItem
//     {
//         public int Id { get; set; }

//         [ForeignKey("Order")]
//         public int OrderId { get; set; }
//         [JsonIgnore] public Order? Order { get; set; }
//         [ForeignKey("Product")]
//         public int ProductId { get; set; }
//         public Product? Product { get; set; }

//         public int Quantity { get; set; }
//         public decimal PriceAtPurchase { get; set; }
//         public string? VendorId { get; set; }          // AspNetUsers.Id
//         public decimal CommissionAmount { get; set; } // platform cut
//         public decimal VendorEarnings { get; set; }   // amount vendor receives
//     }

//     public enum OrderStatus
//     {
//         Draft = 0,
//         PendingPayment = 1,
//         Paid = 2,
//         Processing = 3,
//         Shipped = 4,
//         Delivered = 5,
//         Cancelled = 6,
//         Refunded = 7
//     }

//     public class OrderPayment
//     {
//         public int Id { get; set; }

//         [ForeignKey("Order")]
//         public int OrderId { get; set; }
//         [JsonIgnore]
//                 public Order? Order { get; set; }
//                 public string? Provider { get; set; }
//         public string? TransactionId { get; set; }
//         public decimal Amount { get; set; }
//         public string Currency { get; set; } = "EGBP";
//         public string? Status { get; set; }
//         public DateTime? PaidAt { get; set; }
//     }

//     public class OrderShipping
//     {
//         public int Id { get; set; }

//         [ForeignKey("Order")]
//         public int OrderId { get; set; }
//         [JsonIgnore]
//                 public Order? Order { get; set; }
//                 public string? RecipientName { get; set; }
//         public string? Phone { get; set; }
//         public string? AddressLine1 { get; set; }
//         public string? AddressLine2 { get; set; }
//         public string? City { get; set; }
//         public string? State { get; set; }
//         public string? PostalCode { get; set; }
//         public string? Country { get; set; }
//         public string? TrackingNumber { get; set; }
//         public string? Carrier { get; set; }
//         public DateTime? ShippedAt { get; set; }
//         public DateTime? DeliveredAt { get; set; }
//     }
// }



// ─────────────────────────────────────────────────────────────────────────────
// Domain aggregate. Business rules live HERE, not in the controller or service.
// TransitionTo enforces the legal state machine so no code path can put an
// order into an illegal state (e.g. Shipped → PendingPayment).
// ─────────────────────────────────────────────────────────────────────────────
 
namespace Myshop.Core.Models
{
    public enum OrderStatus
    {
        Draft           = 0,
        PendingPayment  = 1,
        Paid            = 2,
        Processing      = 3,
        Shipped         = 4,
        Delivered       = 5,
        Cancelled       = 6,
        Refunded        = 7
    }
 
    public class Order
    {
        public int    Id         { get; set; }
        public string? UserId    { get; set; }       // AppUser.Id (JWT subject)
        public int?   CustomerId { get; set; }        // Customer profile FK (nullable)
 
        public OrderStatus Status    { get; set; } = OrderStatus.Draft;
        public decimal     Total     { get; set; }
        public string      Currency  { get; set; } = "EGP";
 
        // Stored at order time so the email service never needs a re-query
        public string? CustomerEmail { get; set; }
        public string? CustomerName  { get; set; }
 
        public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
 
        // ── Navigation ────────────────────────────────────────────────────────
        public AppUser?              AppUser  { get; set; }
        public ICollection<OrderItem> Items   { get; set; } = new List<OrderItem>();
        public OrderPayment?          Payment  { get; set; }
        public OrderShipping?         Shipping { get; set; }
 
        // ── State machine ─────────────────────────────────────────────────────
        // All legal transitions. Throw on illegal ones so the bug surfaces
        // immediately at the domain layer, not silently in the DB.
        private static readonly Dictionary<OrderStatus, OrderStatus[]> _allowedTransitions = new()
        {
            [OrderStatus.Draft]          = [OrderStatus.PendingPayment, OrderStatus.Cancelled],
            [OrderStatus.PendingPayment] = [OrderStatus.Paid, OrderStatus.Cancelled, OrderStatus.Refunded],
            [OrderStatus.Paid]           = [OrderStatus.Processing, OrderStatus.Refunded, OrderStatus.Cancelled],
            [OrderStatus.Processing]     = [OrderStatus.Shipped, OrderStatus.Cancelled],
            [OrderStatus.Shipped]        = [OrderStatus.Delivered],
            [OrderStatus.Delivered]      = [OrderStatus.Refunded],
            [OrderStatus.Cancelled]      = [],
            [OrderStatus.Refunded]       = [],
        };
 
        public void TransitionTo(OrderStatus next)
        {
            if (!_allowedTransitions.TryGetValue(Status, out var allowed) || !allowed.Contains(next))
                throw new InvalidOperationException(
                    $"Cannot transition order #{Id} from {Status} to {next}.");
 
            Status    = next;
            UpdatedAt = DateTime.UtcNow;
        }
    }
 
    public class OrderItem
    {
        public int    Id      { get; set; }
        public int    OrderId { get; set; }
        public Order? Order   { get; set; }
 
        public int      ProductId { get; set; }
        public Product? Product   { get; set; }
 
        // Frozen at checkout — never recalculate from Product.Price after the fact
        public string  ProductName    { get; set; } = string.Empty;
        public decimal PriceAtPurchase { get; set; }
        public int     Quantity       { get; set; }
 
        // Copied from Product.VendorId at checkout so vendor queries never
        // need a Product JOIN just to find their items
        public string? VendorId { get; set; }
    }
 
    public class OrderPayment
    {
        public int    Id      { get; set; }
        public int    OrderId { get; set; }
        public Order? Order   { get; set; }
 
        public string  Provider      { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public decimal Amount        { get; set; }
        public string  Status        { get; set; } = "Pending";   // Pending / Paid / Failed / Refunded
        public string  Currency      { get; set; } = "EGP";
        public DateTime? PaidAt      { get; set; }
    }
 
    public class OrderShipping
    {
        public int    Id      { get; set; }
        public int    OrderId { get; set; }
        public Order? Order   { get; set; }
 
        public string? RecipientName { get; set; }
        public string? Phone        { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City         { get; set; }
        public string? State        { get; set; }
        public string? PostalCode   { get; set; }
        public string? Country      { get; set; }
 
        // Set by vendor on ship action
        public string?   TrackingNumber { get; set; }
        public string?   Carrier        { get; set; }
        public DateTime? ShippedAt      { get; set; }
        public DateTime? DeliveredAt    { get; set; }
    }
}