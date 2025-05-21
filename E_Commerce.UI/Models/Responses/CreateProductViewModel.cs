using E_Commerce.UI.Models.Requests;

namespace E_Commerce.UI.Models.Responses
{
    public class CreateProductViewModel
    {
        public ProductResponseDto Product { get; set; } = new(); 
        public IEnumerable<CategoryResponseDto> Categories { get; set; } = new List<CategoryResponseDto>();
        public IEnumerable<BrandResponseDto> Brands { get; set; } = new List<BrandResponseDto>();
    }
}
