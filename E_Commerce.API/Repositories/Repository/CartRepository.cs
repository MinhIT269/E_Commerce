using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        public CartRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)!
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<bool> AddToCartAsync(Cart cart)
        {
            if (cart == null) return false;
            _context.Carts.Add(cart);
            return await SaveChangesAsync();
        }
        public async Task<bool> UpdateCartItemQuantityAsync(string userId, List<CartItemUpdateDto> items)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            bool updated = false;

            foreach (var item in items)
            {
                var cartItem = cart.CartItems!.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                    updated = true;
                }
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return updated;
        }
        public async Task<bool> RemoveFromCartAsync(Guid cartItemId, string userId)
        {
            var cartItem = await _context.CartItem
              .FirstOrDefaultAsync(x => x.CartItemId == cartItemId && x.Cart!.UserId == userId);
            if (cartItem == null) return false;
            _context.CartItem.Remove(cartItem);
            return await SaveChangesAsync();
        }
        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return false;
            _context.Carts.Remove(cart);
            return await SaveChangesAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
