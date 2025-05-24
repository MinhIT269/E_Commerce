using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface ICartService
    {
        Task<CartResponseDto?> GetCartByUserIdAsync(string userId);
        Task<bool> AddToCartAsync(AddToCartRequestDto cart);
        Task<bool> UpdateQuantityAsync(UpdateCartItemRequestDto requestDto);
        Task<bool> RemoveFromCartAsync(RemoveCartItemRequestDto requestDto);
        Task<bool> ClearCartAsync(string userId);
    }
}
