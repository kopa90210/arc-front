using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;

using Myshop.Core.Interfaces;
using Myshop.Application.Interfaces;    


namespace Myshop.Application.Services
{
    public class VendorService : IVendorService
    {
         private readonly IVendorContext _vendor;
         private readonly IVendorRepo _vendorRepo;
        public VendorService(IVendorRepo vendorRepo, IVendorContext vendor) { _vendorRepo = vendorRepo; _vendor = vendor;}
        // private readonly AppDbContext _db;
         
        // public VendorService(AppDbContext db) { _db = db;  }

        public async Task<VendorProfile?> GetMyProfileAsync()
    {
        return await _vendorRepo.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == _vendor.UserId);
    }

        public async Task<VendorProfile?> GetByUserIdAsync(string userId)
            => await _vendorRepo.VendorProfiles.FirstOrDefaultAsync(v => v.UserId == userId);

      public async Task<VendorProfile?> CreateOrUpdateAsync(VendorProfile payload)
{
    var existing = await GetMyProfileAsync();
    
    if (existing == null)
    {
        // Create new
        payload.UserId = _vendor.UserId;
       await  _vendorRepo.AddAsync(payload);
    }
    else
    {
        // Update existing
        existing.StoreName = payload.StoreName;
        existing.StoreLogo = payload.StoreLogo;
        existing.StoreDescription = payload.StoreDescription;
        existing.Address = payload.Address;
    }
    
    await _vendorRepo.SaveChangesAsync();
    return existing ?? payload;
}

        public async Task<IEnumerable<VendorProfile>> GetAllAsync()
            => await _vendorRepo.VendorProfiles.AsNoTracking().ToListAsync();
    }
}
