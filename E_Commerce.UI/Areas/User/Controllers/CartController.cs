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
        private readonly ApiRequestHelper _apiRequestHelper;
        public CartController(ApiRequestHelper apiRequestHelper)
        {
            _apiRequestHelper = apiRequestHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
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

            request.UserId = userId;

            var apiResponse = await _apiRequestHelper
                .SendPostRequestAsync<ApiMessageResponse>("/api/cart/add", request);

            if (apiResponse == null)
                return BadRequest(new { message = "Thêm giỏ hàng thất bại." });

            return Ok(new { message = apiResponse.Message });
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] RemoveFromCartRequestDto dto)
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Bạn phải đăng nhập." });

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            dto.UserId = userId!;

            var apiResponse = await _apiRequestHelper
                .SendDeleteRequestAsync<ApiMessageResponse>("/api/cart/remove", dto);

            if (apiResponse != null)
                return Ok(new { message = apiResponse.Message });

            return BadRequest(new { message = "Xóa sản phẩm thất bại." });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Bạn phải đăng nhập." });
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var apiResponse = await _apiRequestHelper
                .SendGetRequestAsync<CartResponseDto>($"/api/cart/{userId}");
            if (apiResponse != null)
                return Ok(apiResponse);
            return NotFound(new { message = "Không tìm thấy giỏ hàng." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItems([FromBody] UpdateCartItemsRequestDto request)
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Bạn phải đăng nhập."});

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Không xác định được người dùng." });

            request.UserId = userId;

            var apiResponse = await _apiRequestHelper
                .SendPutRequestAsync<ApiMessageResponse>("/api/cart/update", request);

            if (apiResponse != null)
                return Ok(new { message = apiResponse.Message });

            return BadRequest(new { message = "Cập nhật giỏ hàng thất bại." });
        }
    }
}
