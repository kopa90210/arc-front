namespace Myshop.Core.Models
{
    public class OutfitBoard
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? User { get; set; }
        public ICollection<OutfitBoardItem> Items { get; set; } = new List<OutfitBoardItem>();
    }
}
