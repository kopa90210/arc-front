using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Myshop.Core.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // [ForeignKey("AppUser")]
        public string? UserId { get; set; } 
        public AppUser? AppUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int Id { get; set; }

        // [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; } = 1;
    }
}



