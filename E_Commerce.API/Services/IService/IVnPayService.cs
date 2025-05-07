using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext httpContext, VnPaymentRequestDto model);
        VnPaymentResponseDto PaymentExeCute(IQueryCollection collection);
    }
}
