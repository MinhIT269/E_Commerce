namespace E_Commerce.API.Models.Responses
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new();
    }
}
