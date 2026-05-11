namespace Myshop.Core.Models
{
    public class OutfitItem
    {
        public int Id { get; set; }
        public int OutfitId { get; set; }
        public string ItemType { get; set; } = string.Empty; // "UserItem" or "Product"
        public int ItemId { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public int LayerOrder { get; set; }

        public Outfit? Outfit { get; set; }
    }
}
