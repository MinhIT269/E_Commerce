using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IPurchaseService
    {
        Task<List<OrderDto>> GetFilteredOrders(int page, int pageSize, string seachQuery, string sortCriteria);
        Task<int> GetTotalOrdersAsync(string searchQuery, string sortCriteria);
        Task<object> GetPaymentMethodStatisticsAsync();
    }
}
