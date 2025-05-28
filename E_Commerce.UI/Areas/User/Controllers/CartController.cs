using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly ApiRequestHelper _api;
        public CartController(ApiRequestHelper apiRequestHelper) => _api = apiRequestHelper;

        private string? GetUserIdFromToken()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token)) return null;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized(new { message = "Bạn phải đăng nhập." });

            request.UserId = userId;
            var res = await _api.SendPostRequestAsync<ApiMessageResponse>("/api/cart/add", request);
            return res == null
                ? BadRequest(new { message = "Thêm giỏ hàng thất bại." })
                : Ok(new { message = res.Message });
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] RemoveFromCartRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized(new { message = "Bạn phải đăng nhập." });

            dto.UserId = userId;
            var res = await _api.SendDeleteRequestAsync<ApiMessageResponse>("/api/cart/remove", dto);
            return res != null
                ? Ok(new { message = res.Message })
                : BadRequest(new { message = "Xóa sản phẩm thất bại." });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized(new { message = "Bạn phải đăng nhập." });

            var res = await _api.SendGetRequestAsync<CartResponseDto>($"/api/cart/{userId}");
            return res != null
                ? Ok(res)
                : NotFound(new { message = "Không tìm thấy giỏ hàng." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItems([FromBody] UpdateCartItemsRequestDto request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized(new { message = "Bạn phải đăng nhập." });

            request.UserId = userId;
            var res = await _api.SendPutRequestAsync<ApiMessageResponse>("/api/cart/update", request);
            return res != null
                ? Ok(new { message = res.Message })
                : BadRequest(new { message = "Cập nhật giỏ hàng thất bại." });
        }
    }
}
