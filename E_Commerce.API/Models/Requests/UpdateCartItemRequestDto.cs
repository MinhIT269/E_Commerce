namespace E_Commerce.API.Models.Requests
{
    public class UpdateCartItemRequestDto
    {
        public string UserId { get; set; } = null!;
        public List<CartItemUpdateDto> Items { get; set; } = new();
    }
}
