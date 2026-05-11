namespace Myshop.Core.Dtos
{
    // ── Inbound: checkout ─────────────────────────────────────────────────────
    public class CheckoutOrderDto
    {
        public int?   CustomerId    { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Currency      { get; set; }
        public ShippingDto?                Shipping { get; set; }
        public List<CheckoutOrderItemDto>  Items    { get; set; } = new();
    }
 
    public class CheckoutOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity  { get; set; } = 1;
    }
 
    public class ShippingDto
    {
        public string? RecipientName { get; set; }
        public string? Phone        { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City         { get; set; }
        public string? State        { get; set; }
        public string? PostalCode   { get; set; }
        public string? Country      { get; set; }
    }
 
    // ── Inbound: payment callbacks ────────────────────────────────────────────
    public class PaymentSuccessDto
    {
        public string? Provider      { get; set; }
        public string? TransactionId { get; set; }
    }
 
    public class PaymentFailedDto
    {
        public string? Provider      { get; set; }
        public string? TransactionId { get; set; }
    }
 
    // ── Inbound: vendor ships ──────────────────────────────────────────────────
    public class ShipOrderDto
    {
        public string  TrackingNumber    { get; set; } = string.Empty;
        public string  Carrier           { get; set; } = string.Empty;
        public string  EstimatedDelivery { get; set; } = "3-5 business days";
    }
 
    // ── Outbound: full result (returned after mutations) ─────────────────────
    public class OrderResultDto
    {
        public int    Id       { get; set; }
        public string Status   { get; set; } = string.Empty;
        public decimal Total   { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Message  { get; set; } = string.Empty;     // human-readable for Cart.jsx toast
    }
 
    // ── Outbound: list view (used in my-orders, vendor dashboard, admin) ─────
    public class OrderSummaryDto
    {
        public int      Id        { get; set; }
        public string   Status    { get; set; } = string.Empty;
        public decimal  Total     { get; set; }
        public string   Currency  { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int      ItemCount { get; set; }
        public string?  CustomerEmail { get; set; }
        public string?  TrackingNumber { get; set; }
    }

    // ── Outbound: admin-focused list view ────────────────────────────────────
    public class OrderAdminSummaryDto
    {
        public int      Id        { get; set; }
        public string   Customer  { get; set; } = string.Empty;
        public decimal  Total     { get; set; }
        public string   Status    { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int      ItemCount { get; set; }
    }

    public class RevenueByDayDto
    {
        public string  Day       { get; set; } = string.Empty;
        public int     DayOfWeek { get; set; }
        public decimal Revenue   { get; set; }
    }

    public class TopProductDto
    {
        public int    Rank      { get; set; }
        public int    ProductId { get; set; }
        public string Name      { get; set; } = string.Empty;
        public int    Units     { get; set; }
    }
}