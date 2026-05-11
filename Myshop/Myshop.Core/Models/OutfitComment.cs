namespace Myshop.Core.Models
{
    public class OutfitComment
    {
        public int Id { get; set; }
        public int OutfitId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Outfit? Outfit { get; set; }
        public AppUser? User { get; set; }
    }
}
