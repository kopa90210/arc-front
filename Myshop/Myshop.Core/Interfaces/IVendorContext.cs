
// namespace Myshop.Api.Security
// {
// public interface IVendorContext
// {
//     string UserId { get; }
//     int VendorId { get;us }
// }
// }
using Microsoft.EntityFrameworkCore;    
using Myshop.Core.Models;
using Myshop.Core.Interfaces;
namespace Myshop.Core.Interfaces  
{  
public interface IVendorContext
{
     string UserId { get; }
   int VendorId { get; }
    DbSet<VendorProfile> VendorProfiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
}