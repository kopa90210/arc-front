using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;

namespace Myshop.Infrastructure.Context
{
    public class AppDbContext : IdentityDbContext<AppUser> 
    {
//         public class AppDbContext : DbContext, IVendorContext
// {
   
// }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        //  public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
       public DbSet<OrderPayment>   Payments { get; set; }
       public DbSet<OrderShipping>  ShippingInfos { get; set; }
       public DbSet<VendorProfile>  VendorProfiles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // public DbSet<Vendor> Vendors { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<Outfit> Outfits { get; set; }
        public DbSet<OutfitItem> OutfitItems { get; set; }
        public DbSet<OutfitLike> OutfitLikes { get; set; }
        public DbSet<OutfitComment> OutfitComments { get; set; }
        public DbSet<Selfie> Selfies { get; set; }
        public DbSet<SelfieLike> SelfieLikes { get; set; }
        public DbSet<SelfieComment> SelfieComments { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<OutfitBoard> OutfitBoards { get; set; }
        public DbSet<OutfitBoardItem> OutfitBoardItems { get; set; }
        public DbSet<OutfitCollection> OutfitCollections { get; set; }
        public DbSet<CollectionItem>   CollectionItems   { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent cascade cycle on CollectionItems -> Outfits
            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.Outfit)
                .WithMany()
                .HasForeignKey(ci => ci.OutfitId)
                .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<VendorProfile>()
            //     .HasIndex(v => v.UserId)
            //     .IsUnique();

            // modelBuilder.Entity<VendorProfile>()
            //     .Property(v => v.Balance)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<VendorProfile>()
            //     .Property(v => v.TotalSales)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<OrderItem>()
            //     .Property(oi => oi.PriceAtPurchase)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<OrderItem>()
            //     .Property(oi => oi.CommissionAmount)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<OrderItem>()
            //     .Property(oi => oi.VendorEarnings)
            //     .HasPrecision(18, 2);

            // base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // modelBuilder.Entity<Order>()
            //     .Property(o => o.Total)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<Payment>()
            //     .Property(p => p.Amount)
            //     .HasPrecision(18, 2);

            // modelBuilder.Entity<Order>()
            //     .HasOne(o => o.Payment)
            //     .WithOne(p => p.Order)
            //     .HasForeignKey<Payment>(p => p.OrderId);

            // modelBuilder.Entity<Order>()
            //     .HasOne(o => o.Shipping)
            //     .WithOne(s => s.Order)
            //     .HasForeignKey<ShippingInfo>(s => s.OrderId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.AppUser)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserItem>()
                .HasOne(ui => ui.User)
                .WithMany()
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserItem>()
                .Property(ui => ui.Name)
                .HasMaxLength(200);

            modelBuilder.Entity<UserItem>()
                .Property(ui => ui.Category)
                .HasMaxLength(100);

            modelBuilder.Entity<UserItem>()
                .Property(ui => ui.Color)
                .HasMaxLength(80);

            modelBuilder.Entity<UserItem>()
                .Property(ui => ui.Season)
                .HasMaxLength(80);

            modelBuilder.Entity<UserItem>()
                .Property(ui => ui.Brand)
                .HasMaxLength(120);

            modelBuilder.Entity<UserItem>()
                .HasIndex(ui => ui.UserId);

            modelBuilder.Entity<Outfit>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Outfit>()
                .Property(o => o.Name)
                .HasMaxLength(150);

            modelBuilder.Entity<Outfit>()
                .Property(o => o.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Outfit>()
                .Property(o => o.Tags)
                .HasMaxLength(400);

            modelBuilder.Entity<Outfit>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<OutfitItem>()
                .HasOne(oi => oi.Outfit)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OutfitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutfitItem>()
                .Property(oi => oi.ItemType)
                .HasMaxLength(20);

            modelBuilder.Entity<OutfitItem>()
                .HasIndex(oi => oi.OutfitId);

            modelBuilder.Entity<OutfitLike>()
                .HasOne(ol => ol.Outfit)
                .WithMany()
                .HasForeignKey(ol => ol.OutfitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutfitLike>()
                .HasOne(ol => ol.User)
                .WithMany()
                .HasForeignKey(ol => ol.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OutfitLike>()
                .HasIndex(ol => new { ol.OutfitId, ol.UserId })
                .IsUnique();

            modelBuilder.Entity<OutfitComment>()
                .HasOne(oc => oc.Outfit)
                .WithMany()
                .HasForeignKey(oc => oc.OutfitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutfitComment>()
                .HasOne(oc => oc.User)
                .WithMany()
                .HasForeignKey(oc => oc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OutfitComment>()
                .Property(oc => oc.Text)
                .HasMaxLength(500);

            modelBuilder.Entity<OutfitComment>()
                .HasIndex(oc => oc.OutfitId);

            modelBuilder.Entity<Selfie>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Selfie>()
                .Property(s => s.Caption)
                .HasMaxLength(500);

            modelBuilder.Entity<Selfie>()
                .Property(s => s.ImageUrl)
                .HasMaxLength(400);

            modelBuilder.Entity<Selfie>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<SelfieLike>()
                .HasOne(sl => sl.Selfie)
                .WithMany()
                .HasForeignKey(sl => sl.SelfieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SelfieLike>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SelfieLike>()
                .HasIndex(sl => new { sl.SelfieId, sl.UserId })
                .IsUnique();

            modelBuilder.Entity<SelfieComment>()
                .HasOne(sc => sc.Selfie)
                .WithMany()
                .HasForeignKey(sc => sc.SelfieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SelfieComment>()
                .HasOne(sc => sc.User)
                .WithMany()
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SelfieComment>()
                .Property(sc => sc.Text)
                .HasMaxLength(500);

            modelBuilder.Entity<SelfieComment>()
                .HasIndex(sc => sc.SelfieId);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany()
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany()
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasIndex(uf => new { uf.FollowerId, uf.FollowingId })
                .IsUnique();

            modelBuilder.Entity<OutfitBoard>()
                .HasOne(ob => ob.User)
                .WithMany()
                .HasForeignKey(ob => ob.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OutfitBoard>()
                .Property(ob => ob.Name)
                .HasMaxLength(120);

            modelBuilder.Entity<OutfitBoard>()
                .HasIndex(ob => new { ob.UserId, ob.Name })
                .IsUnique();

            modelBuilder.Entity<OutfitBoardItem>()
                .HasOne(obi => obi.OutfitBoard)
                .WithMany(ob => ob.Items)
                .HasForeignKey(obi => obi.OutfitBoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutfitBoardItem>()
                .HasOne(obi => obi.Outfit)
                .WithMany()
                .HasForeignKey(obi => obi.OutfitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutfitBoardItem>()
                .HasIndex(obi => new { obi.OutfitBoardId, obi.OutfitId })
                .IsUnique();
// ── Order ──────────────────────────────────────────────
            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o => o.Total).HasColumnType("decimal(18,2)");
                e.Property(o => o.Currency).HasMaxLength(10).HasDefaultValue("EGP");
                e.Property(o => o.Status).HasConversion<int>();
 
                e.HasMany(o => o.Items)
                 .WithOne(i => i.Order)
                 .HasForeignKey(i => i.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
 
                e.HasOne(o => o.Payment)
                 .WithOne(p => p.Order)
                 .HasForeignKey<OrderPayment>(p => p.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
 
                e.HasOne(o => o.Shipping)
                 .WithOne(s => s.Order)
                 .HasForeignKey<OrderShipping>(s => s.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
 
            // ── OrderItem ──────────────────────────────────────────
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.PriceAtPurchase).HasColumnType("decimal(18,2)");
                e.Property(i => i.ProductName).HasMaxLength(200);
                e.Property(i => i.VendorId).HasMaxLength(450);
 
                // Soft FK to Product — no cascade so deleting a product
                // doesn't destroy historical order data
                e.HasOne(i => i.Product)
                 .WithMany()
                 .HasForeignKey(i => i.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
 
            // ── Payment ────────────────────────────────────────────
            modelBuilder.Entity<OrderPayment>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                e.Property(p => p.Provider).HasMaxLength(50);
                e.Property(p => p.TransactionId).HasMaxLength(200);
            });
 
            // ── ShippingInfo ───────────────────────────────────────
            modelBuilder.Entity<OrderShipping>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.RecipientName).HasMaxLength(150);
                e.Property(s => s.AddressLine1).HasMaxLength(300);
                e.Property(s => s.City).HasMaxLength(100);
                e.Property(s => s.Country).HasMaxLength(100);
                e.Property(s => s.TrackingNumber).HasMaxLength(100);
                e.Property(s => s.Carrier).HasMaxLength(100);
            });
 
            // ── VendorProfile ──────────────────────────────────────
            modelBuilder.Entity<VendorProfile>(e =>
            {
                e.HasKey(v => v.Id);
                e.Property(v => v.Balance).HasColumnType("decimal(18,2)");
                e.Property(v => v.TotalSales).HasColumnType("decimal(18,2)");
 
                e.HasOne(v => v.User)
                 .WithMany()
                 .HasForeignKey(v => v.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });




        }
    }
}
