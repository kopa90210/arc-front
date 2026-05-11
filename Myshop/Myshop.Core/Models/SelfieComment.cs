namespace Myshop.Core.Models
{
    public class SelfieComment
    {
        public int Id { get; set; }
        public int SelfieId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Selfie? Selfie { get; set; }
        public AppUser? User { get; set; }
    }
}
