
using Myshop.Core.Models;

namespace Myshop.Core.Interfaces
{
    public interface IProductRepo
    {
         Task<List<Product>> GetByVendorUserIdAsync(string userId);
    Task<Product> CreateAsync(string userId, Product product);
    Task<Product?> UpdateAsync(string userId, int productId, Product product);
    Task DeleteAsync(string userId, int productId);

     IQueryable<Product> Products { get; }
    Task<VendorProfile?> GetVendorProfileAsync(string userId);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
        
    }
}
// {
//     // public interface IProductService
//     // {
//     //     Task GetByVendorUserIdAsync(string userId);

//         // Task<Product> CreateProductAsVendorAsync(string vendorUserId, Product product, IFormFile? image);
//         // Task<Product?> UpdateProductAsync(string vendorUserId, int productId, Product update, IFormFile? image);
//         // Task<IEnumerable<Product>> GetByVendorAsync(string vendorUserId);
//         public interface IProductService
// {

//     // Task<List<Product>> GetVendorByUserId(string userId);      
//    Task<List<Product>> GetByVendorUserIdAsync(string userId);
//     Task<Product> CreateAsync(string userId, Product product);
//     Task<Product?> UpdateAsync(string userId, int productId, Product product);
//     Task DeleteAsync(string userId, int productId);
// };
// }

