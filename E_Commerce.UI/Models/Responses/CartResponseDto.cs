namespace E_Commerce.UI.Models.Responses
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<CartItemResponseDto> CartItems { get; set; } = new();
        public decimal TotalPrice => CartItems.Sum(item => item.Total);
    }
}
