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
    }
}
