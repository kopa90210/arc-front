using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;
using Myshop.Infrastructure.Context;
using Myshop.Core.Interfaces;

namespace Myshop.Infrastructure.Repo
{
    public class VendorRepo : IVendorRepo
    {
        private readonly AppDbContext _db;
        private readonly IVendorContext _vendor;

        public VendorRepo(AppDbContext db, IVendorContext vendor)
        {
            _db = db;
            _vendor = vendor;
        }

        // ── IVendorRepo ──────────────────────────────────────────────────────

        public IQueryable<VendorProfile> VendorProfiles => _db.VendorProfiles;

        public async Task<VendorProfile?> GetByUserIdAsync(string userId) =>
            await _db.VendorProfiles
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);

        public async Task<VendorProfile?> GetMyProfileAsync()
        {
            var userId = _vendor.UserId;
            if (string.IsNullOrEmpty(userId)) return null;
            return await _db.VendorProfiles
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<VendorProfile?> CreateOrUpdateAsync(VendorProfile payload)
        {
            var existing = await _db.VendorProfiles
                .FirstOrDefaultAsync(v => v.UserId == payload.UserId);

            if (existing is null)
            {
                _db.VendorProfiles.Add(payload);
                await _db.SaveChangesAsync();
                return payload;
            }

            existing.StoreName        = payload.StoreName;
            existing.StoreLogo        = payload.StoreLogo;
            existing.StoreDescription = payload.StoreDescription;
            existing.Address          = payload.Address;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<VendorProfile>> GetAllAsync() =>
            await _db.VendorProfiles.Include(v => v.User).ToListAsync();

        public async Task AddAsync(VendorProfile profile)
        {
            await _db.VendorProfiles.AddAsync(profile);
        }

        public async Task SaveChangesAsync() =>
            await _db.SaveChangesAsync();

        public async Task<VendorDto?> GetByIdAsync(string vendorId)
        {
            return await _db.VendorProfiles
                .Where(v => v.UserId == vendorId)
                .Select(v => new Myshop.Core.Interfaces.VendorDto
                {
                    UserId = v.UserId,
                    Email = v.User.Email,
                    StoreName = v.StoreName,
                    StoreLogo = v.StoreLogo,
                    Balance = v.Balance,
                    TotalSales = v.TotalSales
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateBalanceAsync(string vendorId, decimal amountToAdd)
        {
            var vendor = await _db.VendorProfiles.FirstOrDefaultAsync(v => v.UserId == vendorId);
            if (vendor != null)
            {
                vendor.Balance += amountToAdd;
                vendor.TotalSales += amountToAdd;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<VendorProfile?> GetProfileAsync(string userId)
        {
            return await _db.VendorProfiles
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }
    }
}
