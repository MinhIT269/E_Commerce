using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;

namespace E_Commerce.API.Services.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<CartResponseDto?> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return cart == null ? null : _mapper.Map<CartResponseDto>(cart);
        }

        public async Task<bool> AddToCartAsync(AddToCartRequestDto requestDto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(requestDto.UserId);
            
            if (cart == null)
            {
                cart = _mapper.Map<Cart>(requestDto);
                return await _cartRepository.AddToCartAsync(cart);
            }
            var existingItem = cart.CartItems?.FirstOrDefault(x => x.ProductId == requestDto.ProductId);
            if (existingItem != null)
            {
                // Nếu sản phẩm đã có trong giỏ, chỉ cập nhật số lượng
                existingItem.Quantity += requestDto.Quantity;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ, tạo mới CartItem
                var newCartItem = _mapper.Map<CartItem>(requestDto); // Ánh xạ từ DTO sang CartItem
                cart.CartItems?.Add(newCartItem);
            }

            return await _cartRepository.SaveChangesAsync();
        }

        public Task<bool> RemoveFromCartAsync(RemoveCartItemRequestDto requestDto)
        {
            return _cartRepository.RemoveFromCartAsync(requestDto.CartItemId, requestDto.UserId);
        }
        public async Task<bool> UpdateQuantityAsync(UpdateCartItemRequestDto requestDto)
        {
            return await _cartRepository.UpdateCartItemQuantityAsync(requestDto.UserId, requestDto.Items);
        }
        public Task<bool> ClearCartAsync(string userId)
        {
            return _cartRepository.ClearCartAsync(userId);
        }
    }
}
