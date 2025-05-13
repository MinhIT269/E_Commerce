using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDto?> GetCategoryByNameAsync(string name);
        Task<bool> CreateCategoryAsync(CategoryRequestDto category);
        Task<bool> UpdateCategoryAsync(Guid id, CategoryRequestDto category);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<(List<CategoryDetailResponseDto> categories, int totalRecords)> GetFilteredCategoriesAsync(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending);
        Task<int> GetTotalCategoriesAsync(string searchQuery);
        Task<bool> IsCategoryNameExistsAsync(string categoryName);
        Task<List<ProductResponseDto>> GetProductsByCategoryIdAsync(Guid categoryId);
    }
}
