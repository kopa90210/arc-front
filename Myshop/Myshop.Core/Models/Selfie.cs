namespace Myshop.Core.Models
{
    public class Selfie
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? User { get; set; }
    }
}
