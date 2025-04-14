using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories!)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages).ToListAsync();
        }

        public async Task<List<Product?>> GetProductsByCategoryAsync(Guid categoryId, int count)
        {
            return await _context.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                .Include(pc => pc.Product)
                .Select(pc => pc.Product)
                .OrderBy(p => p.Quantity)
                .Take(count)
                .ToListAsync();
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Brand)
                .Include(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages);
        }

        public IQueryable<Product> FilterBySearchQuery(IQueryable<Product> query, string filterQuery)
        {
            if (!string.IsNullOrEmpty(filterQuery))
            {
                query = query.Where(p => EF.Functions.Collate(p.Name!, "SQL_Latin1_General_CP1_CI_AI").Contains(filterQuery));
            }
            return query;
        }

        public IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortCriteria, bool isDescending)
        {
            switch (sortCriteria)
            {
                case "category":
                    return isDescending
                        ? query.OrderByDescending(p => p.ProductCategories!.OrderBy(pc => pc.Category!.CategoryName).FirstOrDefault()!.Category!.CategoryName)
                        : query.OrderBy(p => p.ProductCategories!.OrderBy(pc => pc.Category!.CategoryName).FirstOrDefault()!.Category!.CategoryName);
                case "stock":
                    return isDescending ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity);
                case "name":
                    return isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                case "price":
                    return isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                default:
                    return query;
            }
        }

        public IQueryable<Product> ApplyPagination(IQueryable<Product> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public async Task<List<Product>> GetAllAsync(string? filterQuery, string sortBy, bool isAscending,
            int pageNumber = 1, int pageSize = 1000)
        {
            var query = GetAllProducts();
            query = FilterBySearchQuery(query, filterQuery);
            query = ApplySorting(query, sortBy, isAscending);
            query = ApplyPagination(query, pageNumber, pageSize);
            return await query.ToListAsync();
        }

        public async Task<List<Product>> FindProductsByBrandAsync(string filterQuery, Guid brandId)
        {
            return await _context.Products.Include(p => p.Brand)
                .Include(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages)
                .Where(p => EF.Functions.Collate(p.Name!, "SQL_Latin1_General_CP1_CI_AI").Contains(filterQuery) &&
                            p.Brand.BrandId == brandId)
                .ToListAsync();
        }

        public async Task<List<Product>> FindProductsByNameAsync(string name)
        {
            return await _context.Products.Include(p => p.Brand)
                 .Include(p => p.ProductCategories!)
                 .ThenInclude(pc => pc.Category)
                 .Include(p => p.ProductImages)
                 .Where(p => EF.Functions.Collate(p.Name!, "SQL_Latin1_General_CP1_CI_AI").Contains(name))
                 .ToListAsync();
        }

        public async Task<int> CountProductAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}
