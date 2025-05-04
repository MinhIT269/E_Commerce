namespace E_Commerce.UI.Models.Requests
{
    public class UpdateCartItemsRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItemUpdateDto> Items { get; set; } = new();
    }
}
