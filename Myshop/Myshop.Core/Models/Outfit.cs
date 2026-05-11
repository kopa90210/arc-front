namespace Myshop.Core.Models
{
    public class Outfit
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? User { get; set; }
        public ICollection<OutfitItem> Items { get; set; } = new List<OutfitItem>();
    }
}
