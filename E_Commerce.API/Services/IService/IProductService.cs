using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProducts();
        Task<List<ProductResponseDto>?> GetProductsFromQuery(string? filterQuery, string sortBy, bool isAscending, int pageNumber, int pageSize, Guid? categoryId, Guid? brandId);
        Task<List<ProductResponseDto>?> GetProductsFromBrand(Guid brandId, string filterQuery);
        Task<ProductResponseDto?> GetProductByIdAsync(Guid id);
        Task<int> CountProductAsync(string? searchQuery, Guid? categoryId = null, Guid? brandId = null);
        Task<List<ProductResponseDto>?> GetProductsByCategoryAsync(Guid categoryId, int count);
        Task DeleteAsync(Guid id);
        Task<int> GetTotalProduct();
        Task<int> AvailableProducts();
        Task<int> GetLowStockProducts();
        Task<int> GetNewProducts();
        Task<bool> AddProductAsync(ProductRequestDto model, IFormFile mainImage, IList<IFormFile> additionalImages);
        Task<bool> UpdateProductAsync(ProductRequestDto model, IFormFile? mainImage, IList<IFormFile>? additionalImages, List<string>? oldImageUrls);
    }
}
