using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IOrderService
    {
        Task<string> CreateOrderAndPaymentAsync(OrderRequestDto request, HttpContext httpContext);
        Task<PaymentResponseDto> HandlePaymentCallbackAsync(IQueryCollection query);
        Task<List<OrderDto>> GetFilteredOrders(int page, int pageSize, string seachQuery, string sortCriteria, bool isDescending);
        Task<int> GetTotalOrdersAsync(string searchQuery);
        Task<int> TotalOrders();
        Task<int> TotalOrdersSuccess();
        Task<int> TotalOrdersPending();
        Task<decimal> GetTotalAmountOfCompletedOrdersAsync();
        Task<int> TotalOrdersCancel();
        Task<OrderDetailResponseDto> GetOrderDetails(Guid id);
        Task<List<OrderDto>?> GetAllOrders(string id, string? searchQuery, int page = 1, int pageSize = 5);
        Task<int> TotalOrdersByUser(string userId);
        Task<int> TotalOrdersSuccessByUser(string userId);
        Task<int> TotalOrdersPendingByUser(string userId);
        Task<decimal> SumCompletedOrdersAmountByUser(string userId);
        Task<int> CountAllOrdersAsync(string userId, string searchQuery);
        Task<Dictionary<string, decimal>> GetOrderStatistics(string period);
    }
}
