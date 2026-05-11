using Myshop.Core.Models;
// using Myshop.Infrastructure.Repo;
// Ensure VendorProfile is defined in Myshop.Core.Models or add the correct using directive here.

namespace Myshop.Core.Interfaces
{
    public interface IVendorRepo
    {
IQueryable<VendorProfile> VendorProfiles { get; }  // 
        Task<VendorProfile?> GetByUserIdAsync(string UserId);
        Task<VendorProfile?> CreateOrUpdateAsync( VendorProfile payload);
        // Task<IEnumerable<VendorProfile>> GetAllAsync();
           Task<VendorProfile?> GetMyProfileAsync();
    // Task<VendorProfile> SaveMyProfileAsync(VendorProfile profile);
   
    Task<IEnumerable<VendorProfile>> GetAllAsync();
        Task AddAsync(VendorProfile profile);

          Task<VendorDto?>         GetByIdAsync(string userId);
        // Task<List<VendorDto>>    GetAllAsync();
        Task<VendorProfile?>     GetProfileAsync(string userId);
        Task                     UpdateBalanceAsync(string userId, decimal amountToAdd);
    
        //  Task AddAsync(VendorProfile payload);
        
    Task SaveChangesAsync();
    }

    // {
    // public record VendorInfo(
    //     string  UserId,
    //     string? Email,
    //     string? StoreName,
    //     decimal Balance,
    //     decimal TotalSales
    // );
 
    // public interface IVendorRepository
    // {
    //     Task<VendorInfo?> GetByIdAsync(string vendorId);
    //     Task UpdateBalanceAsync(string vendorId, decimal amountToAdd);
    // }

     public class VendorDto
    {
        public string  UserId       { get; set; } = string.Empty;
        public string? Email        { get; set; }   // from AppUser
        public string? StoreName    { get; set; }
        public string? StoreLogo    { get; set; }
        public decimal Balance      { get; set; }
        public decimal TotalSales   { get; set; }
    }
 
    
      

    
}
