using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Add this line



namespace Myshop.Core.Models
{
    public class Product
    {
       public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Stock { get; set; }
        //   public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public string? VendorId { get; set; }
        // ✨ New fields for product enhancements
        public decimal? Rating { get; set; }
        public int? RatingCount { get; set; }
        public decimal? SalePrice { get; set; }
        public bool IsNew { get; set; } = false;
        public string   Status      { get; set; } = "Active";  // "Active" | "Draft"
        public int      Sales       { get; set; } = 0;         // total units sold (updated on order Delivered)
}
        
// public AppUser? User { get; set; }
    }

