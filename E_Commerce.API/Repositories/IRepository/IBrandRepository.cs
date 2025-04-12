using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<Brand?> GetBrandByIdAsync(Guid id);
        Task<Brand?> GetBrandByNameAsync(string name);
        Task<bool> BrandExistsAsync(Guid id);
        Task<bool> BrandExistsByNameAsync(string name);
        Task<bool> CreateBrandAsync(Brand brand);
        Task<bool> UpdateBrandAsync(Brand brand);
        Task<bool> DeleteBrandAsync(Guid id);
        Task<bool> SaveChangesAsync();
    }
}
