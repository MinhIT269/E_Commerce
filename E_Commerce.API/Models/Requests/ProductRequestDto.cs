namespace E_Commerce.API.Models.Requests
{
    public class ProductRequestDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? MetaDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Warranty { get; set; }
        public Guid BrandId { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public List<string>? AdditionalImageUrls { get; set; }
    }
}
