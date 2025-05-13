using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IBrandService
    {
        Task<List<BrandResponseDto>> GetAllBrandsAsync();
        Task<BrandResponseDto?> GetBrandByIdAsync(Guid id);
        Task<BrandResponseDto?> GetBrandByNameAsync(string name);
        Task<bool> CreateBrandAsync(BrandRequestDto brand);
        Task<bool> UpdateBrandAsync(Guid id,BrandRequestDto brand);
        Task<bool> DeleteBrandAsync(Guid id);
        Task<bool> HasProductsByBrandIdAsync(Guid brandId);
        Task<int> GetTotalBrandsAsync(string searchQuery);
        Task<bool> IsBrandNameExists(string brandName);
        Task<(List<BrandDetailResponseDto> brands, int totalRecods)> GetFilteredCategoriesAsync(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending);
    }
}
