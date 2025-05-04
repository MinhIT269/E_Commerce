namespace E_Commerce.API.Models.Requests
{
    public class CartItemUpdateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
