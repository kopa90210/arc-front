

namespace Myshop.Application.Emails.Dtos

{
    // Sent to vendor when a customer places an order
    public record VendorOrderEmailDto(
        string VendorEmail,       // extracted from JWT / vendor profile
        string VendorName,
        int    OrderId,
        string CustomerName,
        decimal Total,
        string Currency,
        List<OrderItemLine> Items,
        DateTime PlacedAt
    );

    // Sent to customer after checkout
    public record OrderConfirmationEmailDto(
        string CustomerEmail,     // extracted from JWT
        string CustomerName,
        int    OrderId,
        decimal Total,
        string Currency,
        string PaymentMethod,
        List<OrderItemLine> Items,
        string ShippingAddress
    );

    // Sent to customer when vendor marks order shipped
    public record ShippingUpdateEmailDto(
        string CustomerEmail,
        string CustomerName,
        int    OrderId,
        string TrackingNumber,
        string Carrier,
        string EstimatedDelivery
    );

    // Marketing / offers / AI recommendations
    public record MarketingEmailDto(
        string CustomerEmail,
        string CustomerName,
        string Subject,
        string HtmlBody           // pre-built HTML — either static offer or AI-generated
    );

    // Shared line item used in order emails
    public record OrderItemLine(
        string ProductName,
        int    Quantity,
        decimal UnitPrice
    );
}