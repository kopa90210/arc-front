namespace Myshop.Application.Dtos
{
    public class VendorProfileDto
    {
        public string? StoreName        { get; set; }
        public string? StoreLogo        { get; set; }
        public string? CoverImage       { get; set; }
        public string? Tagline          { get; set; }
        public string? StoreDescription { get; set; }
        public string? Address          { get; set; }
        public bool    IsVerified       { get; set; }
    }
}