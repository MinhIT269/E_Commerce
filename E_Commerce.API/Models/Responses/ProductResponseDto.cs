namespace E_Commerce.API.Models.Responses
{
    public class ProductResponseDto
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? MetaDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime Hot { get; set; }
        public int Warranty { get; set; }
        public string? BrandName { get; set; }
        public List<string>? CategoryNames { get; set; }
        public List<string>? ProductImages { get; set; }
        public int SoldQuantity { get; set; }
    }
}
