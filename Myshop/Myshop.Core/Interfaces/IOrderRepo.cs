using Myshop.Core.Models;

namespace Myshop.Core.Interfaces
{
    public interface IOrderRepo
    {
        IQueryable<Order> Query { get; }                          // for Include chains
        Task<Order?> GetByIdAsync(int id);                        // with all includes
        Task<Order>  AddAsync(Order order);
        Task         SaveAsync();
        Task         ExecuteInTransactionAsync(Func<Task> work);  // wraps BeginTransaction
    }
    }

