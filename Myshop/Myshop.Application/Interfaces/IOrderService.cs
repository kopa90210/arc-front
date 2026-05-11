using System.Security.Claims;
using Myshop.Core.Models;
using Myshop.Core.Dtos;

namespace Myshop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetMyOrdersAsync(string vendorId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        
        // ── Customer checkout flow ────────────────────────────────────────────
        Task<OrderResultDto> CheckoutAsync(CheckoutOrderDto request, ClaimsPrincipal user);
        Task<OrderResultDto> PaymentSuccessAsync(int orderId, PaymentSuccessDto dto, ClaimsPrincipal user);
        Task<OrderResultDto> PaymentFailedAsync(int orderId, PaymentFailedDto dto, ClaimsPrincipal user);
 
        // ── Customer read ─────────────────────────────────────────────────────
        Task<List<OrderSummaryDto>> GetMyOrdersAsync(ClaimsPrincipal user);
 
        // ── Vendor fulfillment (called from VendorOrdersController) ──────────
        Task<List<OrderSummaryDto>> GetVendorOrdersAsync(string vendorId);
        Task<OrderResultDto> ProcessOrderAsync(int orderId, ClaimsPrincipal vendor);
        Task<OrderResultDto> ShipOrderAsync(int orderId, ShipOrderDto dto, ClaimsPrincipal vendor);
        Task<OrderResultDto> DeliverOrderAsync(int orderId, ClaimsPrincipal vendor);

        //NEW
        Task<List<OrderSummaryDto>> GetVendorOrdersAsync(string vendorId, int limit = 5);
        Task<List<RevenueByDayDto>> GetRevenueChartAsync(string vendorId, string range);
        Task<List<TopProductDto>> GetTopSellingAsync(string vendorId, int top = 3);
 
        // ── Admin ─────────────────────────────────────────────────────────────
        Task<List<OrderSummaryDto>> GetAllOrdersAsync();
    }
}
