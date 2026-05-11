using Microsoft.EntityFrameworkCore;
using Myshop.Core.Models;
using Myshop.Infrastructure.Context;
using Myshop.Core.Interfaces;

namespace Myshop.Infrastructure.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _db;
        public OrderRepo(AppDbContext db) => _db = db;
 
        // Expose IQueryable so the service can chain Include / Where / Select
        public IQueryable<Order> Query => _db.Orders;
 
        public async Task<Order?> GetByIdAsync(int id) =>
            await _db.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .Include(o => o.AppUser)
                .FirstOrDefaultAsync(o => o.Id == id);
 
        public Task<Order> AddAsync(Order order)
        {
            _db.Orders.Add(order);
            return Task.FromResult(order);
        }
 
        public Task SaveAsync() => _db.SaveChangesAsync();
 
        // Wraps a unit of work in a DB transaction.
        // The service calls this for the payment-success flow where stock
        // reduction and status change must be atomic.
        public async Task ExecuteInTransactionAsync(Func<Task> work)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await work();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}