using Myshop.Core.Models;

using Myshop.Application.Interfaces;

namespace Myshop.Application.Interfaces
{
    // public interface IProductService
    // {
    //     Task GetByVendorUserIdAsync(string userId);

        // Task<Product> CreateProductAsVendorAsync(string vendorUserId, Product product, IFormFile? image);
        // Task<Product?> UpdateProductAsync(string vendorUserId, int productId, Product update, IFormFile? image);
        // Task<IEnumerable<Product>> GetByVendorAsync(string vendorUserId);
        public interface IProductService
{

    // Task<List<Product>> GetVendorByUserId(string userId);      
   Task<List<Product>> GetByVendorUserIdAsync(string userId);
    Task<Product> CreateAsync(string userId, Product product);
    Task<Product?> UpdateAsync(string userId, int productId, Product product);
    Task DeleteAsync(string userId, int productId);

     Task<VendorProfile?> GetVendorProfileAsync(string userId);
    // Task<IEnumerable<Product>> GetAllProductsAsync();
    // Task<Product?> GetProductByIdAsync(int id);
    IQueryable<Product> Products { get; }
    // Task AddProductAsync(Product product);
    // Task UpdateProductAsync(Product product);
    // Task DeleteProductAsync(int id);
    Task SaveChangesAsync();
};
}

