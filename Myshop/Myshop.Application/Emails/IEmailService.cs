
using Myshop.Application.Emails.Dtos;


namespace Myshop.Application.Emails
{
    public interface IEmailService
    {
        Task SendVendorOrderNotificationAsync(VendorOrderEmailDto dto);
        Task SendOrderConfirmationAsync(OrderConfirmationEmailDto dto);
        Task SendShippingUpdateAsync(ShippingUpdateEmailDto dto);
        Task SendMarketingEmailAsync(MarketingEmailDto dto);
    }
}