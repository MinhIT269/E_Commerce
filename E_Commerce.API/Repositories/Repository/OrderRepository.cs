using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        public OrderRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.UserInfo)
                .Include(o => o.Promotion)
                .Include(o => o.OrderDetails)!
                    .ThenInclude(od => od.Product)
                .Where(p => p.OrderId == orderId)
                .FirstOrDefaultAsync();

            return order;
        }
        public async Task<List<Order>> GetAllOrderAsync(string? id, string searchQuery)
        {
            if (id != null)
            {
                var query = _context.Orders
                    .Include(x => x.User)
                    .Include(x => x.Promotion)
                    .AsQueryable();

                // Lọc theo UserId
                query = query.Where(x => x.UserId == id);

                // Lọc theo searchQuery (nếu không null hoặc rỗng)
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query = query.Where(order =>
                        order.OrderId.ToString().Substring(0, 8).Contains(searchQuery));
                }

                // Lấy danh sách kết quả
                var orders = await query.Select(order => new Order
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    PromotionId = order.PromotionId ?? Guid.Empty
                }).ToListAsync();

                return orders;
            }

            return new List<Order>();
        }
        public async Task<Order> GetOrderDetailAsync(Guid? id)
        {
            var order = await _context.Orders
                .Include(o => o.User)               
                    .ThenInclude(u => u.UserInfo)
                .Include(o => o.OrderDetails!)      
                    .ThenInclude(od => od.Product)  
                .Include(o => o.Promotion)          
                .Include(o => o.Transaction)       
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order!;
        }
        public async Task<int> TotalOrders()
        {
            return await _context.Orders.CountAsync();
        }
        public async Task<int> TotalOrdersSuccess()
        {
            return await _context.Orders.Where(c => string.Equals(c.Status, "completed", StringComparison.OrdinalIgnoreCase)).CountAsync();
        }
        public async Task<int> TotalOrdersPending()
        {
            return await _context.Orders.Where(c => string.Equals(c.Status, "pending", StringComparison.OrdinalIgnoreCase)).CountAsync();
        }
        public async Task<int> TotalOrdersCancel()
        {
            return await _context.Orders.Where(c => string.Equals(c.Status, "cancel", StringComparison.OrdinalIgnoreCase)).CountAsync();
        }
        public async Task<int> TotalOrdersByUser(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .CountAsync();
        }
        public async Task<int> TotalOrdersSuccessByUser(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId && o.Status!.ToLower() == "completed")
                .CountAsync();
        }

        public async Task<int> TotalOrdersPendingByUser(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId &&
                      (o.Status!.ToLower() == "pending" || o.Status.ToLower() == "cancel"))
                .CountAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
