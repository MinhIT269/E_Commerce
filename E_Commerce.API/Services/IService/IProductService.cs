using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProducts();
        Task<List<ProductResponseDto>> GetProductsFromQuery(string filterQuery, string sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<List<ProductResponseDto>> GetProductsFromBrand(Guid brandId, string filterQuery);
        Task<int> CountProductAsync(string searchQuery);
        Task<List<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId, int count);
    }
}
