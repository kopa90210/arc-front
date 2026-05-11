using Myshop.Core.Models;
// Ensure VendorProfile is defined in Myshop.Core.Models or add the correct using directive here.

namespace Myshop.Application.Services
{
    public interface IVendorService
    {

        Task<VendorProfile?> GetByUserIdAsync(string UserId);
        // Task<VendorProfile?> GetByIdAsync(int id);
        Task<VendorProfile?> CreateOrUpdateAsync( VendorProfile payload);
        // Task<IEnumerable<VendorProfile>> GetAllAsync();
           Task<VendorProfile?> GetMyProfileAsync();
    // Task<VendorProfile> SaveMyProfileAsync(VendorProfile profile);
   
    Task<IEnumerable<VendorProfile>> GetAllAsync();
    }

    
}
