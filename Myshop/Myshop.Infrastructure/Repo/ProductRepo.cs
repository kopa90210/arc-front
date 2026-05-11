using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;
using Myshop.Infrastructure.Context;
using Myshop.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;



namespace Myshop.Infrastructure.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        // private readonly IVendorContext _vendor;
        // private readonly AppDbContext _db;
    // private readonly IVendorContext _vendor;


        public ProductRepo(AppDbContext db, IWebHostEnvironment env) { _db = db; _env = env;}




//  private async Task<Vendor?> GetVendorByUserId(string userId)
//     {
//         return await _db.Vendors
//             .FirstOrDefaultAsync(v => v.UserId == userId);
//     }

   

     public async Task<List<Product>> GetByVendorUserIdAsync(string userId)
        {
            // Find the vendor profile first, then get their products
            var vendorProfile = await _db.VendorProfiles
                .FirstOrDefaultAsync(v => v.UserId == userId);
            
            if (vendorProfile == null)
                return new List<Product>();

            // Assuming VendorId in Product is the UserId
            return await _db.Products
                .Where(p => p.VendorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
   
    

    public async Task<Product> CreateAsync(string userId, Product product)
{
    // Just save the product (image already handled in controller)
    product.VendorId = userId;
    product.CreatedAt = DateTime.UtcNow;

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

    return product;
    }
 public async Task<Product?> UpdateAsync(string userId, int productId, Product productUpdate)
{
    var product = await _db.Products
        .FirstOrDefaultAsync(p => p.Id == productId && p.VendorId == userId);
    
    if (product == null)
        return null;

    // Update fields
    product.Name = productUpdate.Name;
    product.Description = productUpdate.Description;
    product.Price = productUpdate.Price;
    product.Category = productUpdate.Category;
    product.Brand = productUpdate.Brand;
    product.Stock = productUpdate.Stock;
            product.ImageUrl = productUpdate.ImageUrl;

    await _db.SaveChangesAsync();
    return product;
}

    public async Task DeleteAsync(string userId, int productId)
        {
            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.VendorId == userId);
            
            if (product != null)
            {
                _db.Products.Remove(product);
                
            }
        }
        public IQueryable<Product> Products => _db.Products;
        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
        public async Task<VendorProfile?> GetVendorProfileAsync(string userId)
        {
            return await _db.VendorProfiles
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }
        public async Task AddAsync(Product product)
        {
            await _db.Products.AddAsync(product);
        }
}
}

