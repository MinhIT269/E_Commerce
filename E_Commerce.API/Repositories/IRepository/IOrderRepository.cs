using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetAllOrderAsync(string? id, string searchQuery);
        Task<Order> GetOrderDetailAsync(Guid? id);
        Task<int> TotalOrders();
        Task<int> TotalOrdersSuccess();
        Task<int> TotalOrdersPending();
        Task<int> TotalOrdersCancel();
        Task<int> TotalOrdersByUser(string userId);
        Task<int> TotalOrdersSuccessByUser(string userId);
        Task<int> TotalOrdersPendingByUser(string userId);
        Task SaveChangesAsync();
        IQueryable<Order> GetFilteredOrders(string searchQuery, string sortCriteria, bool isDescending);
        Task<decimal> GetTotalAmountOfCompletedOrdersAsync();
        Task<decimal> SumCompletedOrdersAmountByUser(string userId);
    }
}
