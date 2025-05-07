using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction?> GetTransactionByOrderIdAsync(Guid orderId);
    }
}
