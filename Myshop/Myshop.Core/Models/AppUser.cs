using Microsoft.AspNetCore.Identity;

namespace Myshop.Core.Models
{
    public class AppUser : IdentityUser
    {
        // الحقول اللي بتجي تلقائيًا من Identity:
        // Id, UserName, Email, PasswordHash, PhoneNumber, الخ...

        // تقدر تضيف بيانات إضافية هنا
        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImage { get; set; }
         public string?  CoverImage      { get; set; }   // hero banner image
    public string?  Bio             { get; set; }   // short bio text
    public string?  Location        { get; set; }   // "Cairo, Egypt"
    public string?  StyleTagsJson   { get; set; }   // JSON array ["minimalist","streetwear"]
    public string?  SocialLinksJson { get; set; }   // JSON: {"instagram":"handle","tiktok":"handle"}
    public int?     PinnedOutfitId  { get; set; }   // FK → Outfit.Id (nullable)
    public int      TotalLikes      { get; set; }   // denormalized — updated on like toggle
    public int      TotalSaves      { get; set; }   // denormalized — updated on save toggle
    public int      FollowerCount   { get; set; }   // denormalized from Follow table
    public int      FollowingCount  { get; set; }   // denormalized from Follow table
    }
}