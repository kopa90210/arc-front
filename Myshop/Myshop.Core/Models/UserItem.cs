namespace Myshop.Core.Models
{
    public class UserItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Brand { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? User { get; set; }
    }
}
