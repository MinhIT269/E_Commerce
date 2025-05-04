namespace E_Commerce.UI.Models.Requests
{
    public class AddToCartRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
