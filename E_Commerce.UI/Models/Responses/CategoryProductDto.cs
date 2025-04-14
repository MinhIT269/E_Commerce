namespace E_Commerce.UI.Models.Responses
{
    public class CategoryProductDto
    {
        public string? CategoryName { get; set; }
        public List<ProductResponseDto> Products { get; set; } = new List<ProductResponseDto>();
    }
}
