namespace E_Commerce.UI.Models.Responses
{
    public class HomeViewModel
    {
        public List<BrandResponseDto>? Brands { get; set; } 
        public List<CategoryResponseDto>? Categories { get; set; }
        public List<CategoryProductDto> CategoryBlocks { get; set; } = new List<CategoryProductDto>();
    }
}
