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
    }
}
