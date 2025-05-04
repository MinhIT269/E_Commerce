using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<bool> AddToCartAsync(Cart cart);
        Task<bool> RemoveFromCartAsync(Guid cartItemId, string userId);
        Task<bool> UpdateCartItemQuantityAsync(string userId, List<CartItemUpdateDto> items);
        Task<bool> ClearCartAsync(string userId);
        Task<bool> SaveChangesAsync();
    }
}
