using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetAllAsync(string? filterQuery, string sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId, int count);
        Task<List<Product>> FindProductsByBrandAsync(string filterQuery, Guid brandId);
        Task<List<Product>> FindProductsByNameAsync(string name);
        Task<int> CountProductAsync();
    }
}
