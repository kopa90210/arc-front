namespace Myshop.Application.Dtos
{
    public class CreateCollectionDto
    {
        public string Name     { get; set; } = string.Empty;
        public bool   IsPublic { get; set; } = true;
    }

    public class PinOutfitDto
    {
        public int? OutfitId { get; set; }   // null = unpin
    }

    public class SocialLinksDto
    {
        public string? Instagram { get; set; }
        public string? Tiktok   { get; set; }
        public string? Pinterest { get; set; }
    }

    public class UpdateCreatorProfileDto
    {
        public string?       Bio        { get; set; }
        public string?       Location   { get; set; }
        public List<string>? StyleTags  { get; set; }
        public string?       CoverImage { get; set; }
    }
}