using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    { 
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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
            var htmlContent = await _orderService.HandlePaymentCallbackAsync(Request.Query);
            return Content(htmlContent, "text/html");
        }
    }
}
