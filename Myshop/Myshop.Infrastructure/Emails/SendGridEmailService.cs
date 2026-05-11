using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Myshop.Application.Emails;
using Myshop.Application.Emails.Dtos;

namespace Myshop.Infrastructure.Emails
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration config)
        {
            _client    = new SendGridClient(config["SendGrid:ApiKey"]);
            _fromEmail = config["SendGrid:FromEmail"] ?? "noreply@myshop.com";
            _fromName  = config["SendGrid:FromName"]  ?? "MyShop";
        }

        public async Task SendVendorOrderNotificationAsync(VendorOrderEmailDto dto)
        {
            var html = BuildVendorOrderHtml(dto);
            await Send(dto.VendorEmail, dto.VendorName,
                $"New order #{dto.OrderId} — {dto.Total:N2} {dto.Currency}", html);
        }

        public async Task SendOrderConfirmationAsync(OrderConfirmationEmailDto dto)
        {
            var html = BuildOrderConfirmationHtml(dto);
            await Send(dto.CustomerEmail, dto.CustomerName,
                $"Your order #{dto.OrderId} is confirmed", html);
        }

        public async Task SendShippingUpdateAsync(ShippingUpdateEmailDto dto)
        {
            var html = BuildShippingHtml(dto);
            await Send(dto.CustomerEmail, dto.CustomerName,
                $"Your order #{dto.OrderId} has shipped", html);
        }

        public async Task SendMarketingEmailAsync(MarketingEmailDto dto)
        {
            await Send(dto.CustomerEmail, dto.CustomerName, dto.Subject, dto.HtmlBody);
        }

        private async Task Send(string toEmail, string toName, string subject, string html)
        {
            var msg = MailHelper.CreateSingleEmail(
                new EmailAddress(_fromEmail, _fromName),
                new EmailAddress(toEmail, toName),
                subject, null, html);
            await _client.SendEmailAsync(msg);
        }

        // ── Email templates ────────────────────────────────────────────────

        private static string BuildVendorOrderHtml(VendorOrderEmailDto dto)
        {
            var items = string.Join("", dto.Items.Select(i =>
                $"<tr><td style='padding:8px;border-bottom:1px solid #f0e8e0'>{i.ProductName}</td>" +
                $"<td style='padding:8px;border-bottom:1px solid #f0e8e0;text-align:center'>{i.Quantity}</td>" +
                $"<td style='padding:8px;border-bottom:1px solid #f0e8e0;text-align:right'>{i.UnitPrice:N2}</td></tr>"));

            return $"""
            <div style="font-family:'Segoe UI',sans-serif;max-width:580px;margin:0 auto;color:#1e1008">
              <div style="background:#3d2514;padding:24px 28px;border-radius:12px 12px 0 0">
                <h1 style="margin:0;color:#fff;font-size:22px">New order #{dto.OrderId}</h1>
                <p style="margin:6px 0 0;color:rgba(255,255,255,0.75);font-size:14px">
                  Placed {dto.PlacedAt:d MMMM yyyy, HH:mm}
                </p>
              </div>
              <div style="background:#fff;padding:24px 28px;border:1px solid #ede0d4;border-top:none">
                <p style="margin:0 0 16px">Hi {dto.VendorName}, you have a new order from <strong>{dto.CustomerName}</strong>.</p>
                <table style="width:100%;border-collapse:collapse;font-size:14px">
                  <thead>
                    <tr style="background:#f8f4f0">
                      <th style="padding:8px;text-align:left;font-weight:600">Product</th>
                      <th style="padding:8px;text-align:center;font-weight:600">Qty</th>
                      <th style="padding:8px;text-align:right;font-weight:600">Price</th>
                    </tr>
                  </thead>
                  <tbody>{items}</tbody>
                  <tfoot>
                    <tr>
                      <td colspan="2" style="padding:12px 8px;font-weight:700;font-size:16px">Total</td>
                      <td style="padding:12px 8px;font-weight:700;font-size:16px;text-align:right;color:#7a3e1b">
                        {dto.Total:N2} {dto.Currency}
                      </td>
                    </tr>
                  </tfoot>
                </table>
                <p style="margin:20px 0 0;font-size:13px;color:#7a5a46">
                  Log in to your vendor dashboard to process this order.
                </p>
              </div>
            </div>
            """;
        }

        private static string BuildOrderConfirmationHtml(OrderConfirmationEmailDto dto)
        {
            var items = string.Join("", dto.Items.Select(i =>
                $"<tr><td style='padding:8px;font-size:14px'>{i.ProductName} x{i.Quantity}</td>" +
                $"<td style='padding:8px;font-size:14px;text-align:right'>{i.UnitPrice * i.Quantity:N2}</td></tr>"));

            return $"""
            <div style="font-family:'Segoe UI',sans-serif;max-width:580px;margin:0 auto;color:#1e1008">
              <div style="background:#1a6b3a;padding:24px 28px;border-radius:12px 12px 0 0">
                <h1 style="margin:0;color:#fff;font-size:22px">Order confirmed</h1>
                <p style="margin:6px 0 0;color:rgba(255,255,255,0.75)">Order #{dto.OrderId}</p>
              </div>
              <div style="background:#fff;padding:24px 28px;border:1px solid #ede0d4;border-top:none">
                <p style="margin:0 0 16px">Hi {dto.CustomerName}, thank you for your order!</p>
                <table style="width:100%;border-collapse:collapse">
                  <tbody>{items}</tbody>
                  <tfoot>
                    <tr style="border-top:2px solid #ede0d4">
                      <td style="padding:12px 8px;font-weight:700">Total</td>
                      <td style="padding:12px 8px;font-weight:700;text-align:right;color:#7a3e1b">
                        {dto.Total:N2} {dto.Currency}
                      </td>
                    </tr>
                  </tfoot>
                </table>
                <p style="margin:16px 0 0;font-size:13px;color:#5a3a26">
                  Payment: {dto.PaymentMethod} · Ship to: {dto.ShippingAddress}
                </p>
              </div>
            </div>
            """;
        }

        private static string BuildShippingHtml(ShippingUpdateEmailDto dto) => $"""
            <div style="font-family:'Segoe UI',sans-serif;max-width:580px;margin:0 auto;color:#1e1008">
              <div style="background:#185FA5;padding:24px 28px;border-radius:12px 12px 0 0">
                <h1 style="margin:0;color:#fff;font-size:22px">Your order has shipped</h1>
                <p style="margin:6px 0 0;color:rgba(255,255,255,0.75)">Order #{dto.OrderId}</p>
              </div>
              <div style="background:#fff;padding:24px 28px;border:1px solid #ede0d4;border-top:none">
                <p>Hi {dto.CustomerName}, your order is on its way!</p>
                <p style="font-size:14px;color:#5a3a26">
                  Carrier: <strong>{dto.Carrier}</strong><br/>
                  Tracking: <strong>{dto.TrackingNumber}</strong><br/>
                  Estimated delivery: <strong>{dto.EstimatedDelivery}</strong>
                </p>
              </div>
            </div>
            """;
    }
}