using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<CartResponseDto>> GetCartByUserId(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null)
                return NotFound("Cart not found for the given user.");

            return Ok(cart);
        }

        // POST api/cart/add
        [HttpPost("add")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequestDto requestDto)
        {
            var result = await _cartService.AddToCartAsync(requestDto);
            var response = new ApiMessageResponse
            {
                Message = result
                     ? "Product added to cart successfully."
                     : "Failed to add product to cart."
            };
            return result ? Ok(response) : BadRequest(response);
        }

        // DELETE api/cart/remove
        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveFromCart([FromBody] RemoveCartItemRequestDto requestDto)
        {
            var result = await _cartService.RemoveFromCartAsync(requestDto);
            var response = new ApiMessageResponse
            {
                Message = result
                    ? "Item removed from cart."
                    : "Item not found."
            };
            return result ? Ok(response) : BadRequest(response);
        }

        // DELETE api/cart/clear/{userId}
        [HttpDelete("clear/{userId}")]
        public async Task<ActionResult> ClearCart(string userId)
        {
            var result = await _cartService.ClearCartAsync(userId);
            var response = new ApiMessageResponse
            {
                Message = result
                    ? "Cart cleared successfully."
                    : "Cart not found for the given user."
            };
            return result ? Ok(response) : BadRequest(response);
        }

        // PUT api/cart/update
        [HttpPut("update")]
        public async Task<ActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemRequestDto requestDto)
        {
            var result = await _cartService.UpdateQuantityAsync(requestDto);
            var response = new ApiMessageResponse
            {
                Message = result
                    ? "Cập nhật số lượng thành công."
                    : "Không tìm thấy sản phẩm trong giỏ hàng."
            };
            return result ? Ok(response) : BadRequest(response);
        }
    }
}
