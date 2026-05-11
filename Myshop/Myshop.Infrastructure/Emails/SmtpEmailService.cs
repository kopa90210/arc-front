using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Myshop.Application.Emails;
using Myshop.Application.Emails.Dtos;

namespace Myshop.Infrastructure.Emails
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtp;
        private readonly string _from;

        public SmtpEmailService(IConfiguration config)
        {
            _smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"] ?? "587"))
            {
                Credentials = new NetworkCredential(config["Smtp:User"], config["Smtp:Pass"]),
                EnableSsl = true
            };
            _from = config["Smtp:From"] ?? "noreply@myshop.com";
        }

        private Task SendRaw(string to, string subject, string html)
        {
            var msg = new MailMessage(_from, to, subject, html) { IsBodyHtml = true };
            return _smtp.SendMailAsync(msg);
        }

        public Task SendVendorOrderNotificationAsync(VendorOrderEmailDto dto) =>
            SendRaw(dto.VendorEmail, $"New order #{dto.OrderId}", $"<p>Total: {dto.Total}</p>");

        public Task SendOrderConfirmationAsync(OrderConfirmationEmailDto dto) =>
            SendRaw(dto.CustomerEmail, $"Order #{dto.OrderId} confirmed", $"<p>Thank you {dto.CustomerName}!</p>");

        public Task SendShippingUpdateAsync(ShippingUpdateEmailDto dto) =>
            SendRaw(dto.CustomerEmail, $"Order #{dto.OrderId} shipped", $"<p>Tracking: {dto.TrackingNumber}</p>");

        public Task SendMarketingEmailAsync(MarketingEmailDto dto) =>
            SendRaw(dto.CustomerEmail, dto.Subject, dto.HtmlBody);
    }
}
