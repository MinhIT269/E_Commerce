using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetAllAsync(string? filterQuery, string sortBy, bool isAscending, int pageNumber, int pageSize, Guid? categoryId, Guid? brandId);
        Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId, int count);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<List<Product>> FindProductsByBrandAsync(string filterQuery, Guid brandId);
        Task<int> CountProductAsync(string? searchQuery, Guid? categoryId, Guid? brandId);
        Task UpdateProductAsync(Product product);
    }
}
