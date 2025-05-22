using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        public PurchaseRepository(DataContext dbContext)
        {
            _context = dbContext;
        }
        public IQueryable<Order> GetFilteredOrdersPurchase(string searchQuery, string sortCriteria)
        {
            var query = _context.Orders
                .Include(c => c.User)
                .Include(c => c.OrderDetails)!
                .ThenInclude(p => p.Product).AsQueryable();

            // Lọc theo trạng thái "done"
            query = query.Where(c => c.Status == "done");

            // Lọc theo phương thức thanh toán hoặc lấy tất cả
            query = sortCriteria switch
            {
                "MoMo" => query.Where(c => c.Transaction!.PaymentMethod!.Name == "MoMo"),
                "VNPay" => query.Where(c => c.Transaction!.PaymentMethod!.Name == "VnPay"),
                "ZaloPay" => query.Where(c => c.Transaction!.PaymentMethod!.Name == "ZaloPay"),
                "All" or _ => query // Lấy tất cả các đơn hàng "Completed"
            };

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(order =>
                    order.OrderId.ToString().Substring(0, 8).Contains(searchQuery));
            }

            return query;
        }

        public async Task<object> GetPaymentMethodStatisticsAsync()
        {
            var stats = await _context.Orders
                .Where(order => order.Status == "done" && order.Transaction != null && order.Transaction.PaymentMethod != null)
                .GroupBy(order => order.Transaction!.PaymentMethod!.Name)
                .Select(group => new
                {
                    PaymentMethodName = group.Key,
                    OrderCount = group.Count()
                })
                .ToListAsync();

            return stats;
        }
    }
}
