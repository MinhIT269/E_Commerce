using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class CheckOutController : Controller
    {
        ApiRequestHelper _apiRequestHelper;
        public CheckOutController(ApiRequestHelper apiRequestHelper)
        {
            _apiRequestHelper = apiRequestHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto orderRequestDto)
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Bạn phải đăng nhập." });

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims
                             .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                             .Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Không xác định được người dùng." });

            orderRequestDto.UserInfo.UserId = userId;
            var response = await _apiRequestHelper.SendPostRequestAsync<CheckoutResponseDto>("/api/orders/Checkout", orderRequestDto);

            if (!string.IsNullOrEmpty(response?.PaymentUrl))
            {
                return Ok(new { redirect = response.PaymentUrl });
            }
            else
            {
                return BadRequest(new { message = "Đặt hàng thất bại." });
            }
        }

        [HttpGet]
        public IActionResult Result([FromQuery] PaymentResponseDto payment)
        {
            return View(payment);
        }
    }
}
