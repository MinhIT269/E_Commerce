namespace E_Commerce.UI.Models.Requests
{
    public class CartItemUpdateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
