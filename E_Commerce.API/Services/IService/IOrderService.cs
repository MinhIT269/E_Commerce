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
    }
}
