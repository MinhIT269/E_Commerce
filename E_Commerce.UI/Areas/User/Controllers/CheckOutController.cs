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
        private readonly ApiRequestHelper _api;
        public CheckOutController(ApiRequestHelper apiRequestHelper) => _api = apiRequestHelper;

        private string? GetUserIdFromToken()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token)) return null;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Bạn phải đăng nhập." });

            dto.UserInfo.UserId = userId;

            var res = await _api.SendPostRequestAsync<CheckoutResponseDto>("/api/orders/Checkout", dto);
            return !string.IsNullOrEmpty(res?.PaymentUrl)
                ? Ok(new { redirect = res.PaymentUrl })
                : BadRequest(new { message = "Đặt hàng thất bại." });
        }

        [HttpGet]
        public IActionResult Result([FromQuery] PaymentResponseDto payment) => View(payment);
    }
}
