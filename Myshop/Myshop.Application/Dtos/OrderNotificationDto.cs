namespace Myshop.Application.Dtos
{
    public class OrderNotificationDto
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}