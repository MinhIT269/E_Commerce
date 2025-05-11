using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    { 
        private readonly IOrderService _orderService;
        private readonly IConfiguration _config;
        public OrdersController(IOrderService orderService, IConfiguration config)
        {
            _orderService = orderService;
            _config = config;
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderRequestDto request)
        {
            try
            {
                var paymentUrl = await _orderService.CreateOrderAndPaymentAsync(request, HttpContext);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("PaymentBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = await _orderService.HandlePaymentCallbackAsync(Request.Query);
            var frontendBaseUrl = _config["Frontend:BaseUrl"];
            var redirectUrl = $"{frontendBaseUrl}/User/CheckOut/Result?" +
                  $"message={WebUtility.UrlEncode(response.Message)}" +
                  $"&transactionId={response.TransactionId}" +
                  $"&amount={response.Amount}" +
                  $"&statusText={WebUtility.UrlEncode(response.StatusText)}" +
                  $"&statusClass={response.StatusClass}";
            return Redirect(redirectUrl);
        }
    }
}
