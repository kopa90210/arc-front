namespace Myshop.Core.Models
{
    public class OutfitCollection
    {
        public int      Id        { get; set; }
        public string   UserId    { get; set; } = string.Empty;
        public string   Name      { get; set; } = string.Empty;
        public bool     IsPublic  { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public AppUser?                       User  { get; set; }
        public ICollection<CollectionItem>    Items { get; set; } = new List<CollectionItem>();
    }

    public class CollectionItem
    {
        public int Id           { get; set; }
        public int CollectionId { get; set; }
        public int OutfitId     { get; set; }
        public int SortOrder    { get; set; }

        public OutfitCollection? Collection { get; set; }
        public Outfit?           Outfit     { get; set; }
    }
}