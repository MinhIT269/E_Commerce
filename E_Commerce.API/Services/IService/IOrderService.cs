using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IOrderService
    {
        Task<string> CreateOrderAndPaymentAsync(OrderRequestDto request, HttpContext httpContext);
        Task<string> HandlePaymentCallbackAsync(IQueryCollection query);
    }
}
