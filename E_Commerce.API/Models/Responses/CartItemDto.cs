namespace E_Commerce.API.Models.Responses
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }               
        public decimal? PromotionPrice { get; set; }    
        public int Quantity { get; set; }

        public decimal UnitPrice =>
            PromotionPrice.HasValue && PromotionPrice.Value > 0 && PromotionPrice.Value < Price
            ? PromotionPrice.Value
            : Price;

        public decimal Total => UnitPrice * Quantity;
    }
}