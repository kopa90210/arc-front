using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;
using Myshop.Core.Interfaces;

using Myshop.Application.Interfaces;



namespace Myshop.Application.Services
{
    public class ProductService : IProductService
    {
        // private readonly AppDbContext _db;
    //     private readonly IWebHostEnvironment _env;
    //     // private readonly IVendorContext _vendor;
    //     private readonly AppDbContext _db;
    // private readonly IVendorContext _vendor;
    private readonly IProductRepo _Repo;


        public ProductService(IProductRepo repo) { _Repo = repo; }




//  private async Task<Vendor?> GetVendorByUserId(string userId)
//     {
//         return await _db.Vendors
//             .FirstOrDefaultAsync(v => v.UserId == userId);
//     }

   

     public async Task<List<Product>> GetByVendorUserIdAsync(string userId)
        {
            // Find the vendor profile first, then get their products
            var vendorProfile = await _Repo.GetVendorProfileAsync(userId);
            
            if (vendorProfile == null)
                return new List<Product>();

            // Assuming VendorId in Product is the UserId
            return await _Repo.Products
                .Where(p => p.VendorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
   
    

    public async Task<Product> CreateAsync(string userId, Product product)
{
    // Just save the product (image already handled in controller)
    product.VendorId = userId;
    product.CreatedAt = DateTime.UtcNow;

        await _Repo.AddAsync(product);
        await _Repo.SaveChangesAsync();

    return product;
    }
 public async Task<Product?> UpdateAsync(string userId, int productId, Product productUpdate)
{
    var product = await _Repo.Products
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

    await _Repo.SaveChangesAsync();
    return product;
}

    public async Task DeleteAsync(string userId, int productId)
        {
            var product = await _Repo.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.VendorId == userId);
            
            if (product != null)
            {
            //    await _Repo.RemoveAsync(product);
                await _Repo.SaveChangesAsync();
            }
        }

        public async Task<VendorProfile?> GetVendorProfileAsync(string userId)
    => await  _Repo.GetVendorProfileAsync(userId);

public IQueryable<Product> Products => _Repo.Products;

public async Task SaveChangesAsync() => await _Repo.SaveChangesAsync();
}
}

