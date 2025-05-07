using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _context;
        public TransactionRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _context.AddAsync(transaction);
            return transaction;
        }
        public Task<Transaction?> GetTransactionByOrderIdAsync(Guid orderId)
        {
           return _context.Transactions.FirstOrDefaultAsync(t => t.OrderId == orderId);
        }
    }
}
