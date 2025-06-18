namespace E_Commerce.UI.Models.Responses
{
    public class CartItemResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; } 
        public decimal? PromotionPrice { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice
        {
            get
            {
                var promo = PromotionPrice ?? 0;
                if (promo > 0 && (Price == 0 || promo < Price))
                    return promo;

                return Price;
            }
        }

        public decimal Total => UnitPrice * Quantity;
    }
}
