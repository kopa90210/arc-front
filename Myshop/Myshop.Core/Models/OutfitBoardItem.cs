namespace Myshop.Core.Models
{
    public class OutfitBoardItem
    {
        public int Id { get; set; }
        public int OutfitBoardId { get; set; }
        public int OutfitId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OutfitBoard? OutfitBoard { get; set; }
        public Outfit? Outfit { get; set; }
    }
}
