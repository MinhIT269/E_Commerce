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

        public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId, int count)
        {
            return await _context.ProductCategories
                .Where(pc => pc.CategoryId == categoryId && pc.Product != null)
                .Select(pc => pc.Product!)
                .OrderBy(p => p.Quantity)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories!)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Brand)
                .Include(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages);
        }

        public IQueryable<Product> FilterBySearchQuery(IQueryable<Product> query, string? filterQuery)
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
            int pageNumber = 1, int pageSize = 1000, Guid? categoryId = null, Guid? brandId = null)
        {
            var query = GetAllProducts();
            query = FilterBySearchQuery(query, filterQuery);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategories!.Any(pc => pc.CategoryId == categoryId.Value));
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

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
                            p.Brand!.BrandId == brandId)
                .ToListAsync();
        }

        public async Task<int> CountProductAsync(string? searchQuery, Guid? categoryId = null, Guid? brandId = null)
        {
            var query = _context.Products
                                .Include(p => p.Brand)
                                .Include(p => p.ProductCategories)!
                                .ThenInclude(pc => pc.Category)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(p => EF.Functions.Collate(p.Name!, "SQL_Latin1_General_CP1_CI_AI").Contains(searchQuery));

            if (categoryId.HasValue)
                query = query.Where(p => p.ProductCategories!.Any(pc => pc.CategoryId == categoryId.Value));

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            return await query.CountAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<Dictionary<Guid, int>> GetSoldQuantitiesAsync(List<Guid> productIds)
        {
            return await _context.OrderDetail
                .Where(od => productIds.Contains(od.ProductId) && od.Order != null && od.Order.Status == "done")
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(od => od.Quantity)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                if (product.ProductImages != null)
                {
                    foreach (var image in product.ProductImages)
                    {
                        _context.Entry(image).State = EntityState.Detached == _context.Entry(image).State
                            ? EntityState.Added
                            : _context.Entry(image).State;
                    }
                }

                _context.Products.Update(product);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                return false;
            }
        }

        public async Task DeletePromotionAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetAvailableProduct()
        {
            var product = await _context.Products.Where(p => p.Quantity >= 5).CountAsync();
            return product;
        }

        public async Task<int> GetLowStockProducts()
        {
            var product = await _context.Products.Where(p => p.Quantity < 5).CountAsync();
            return product;
        }

        public async Task<int> GetNewProducts()
        {
            var recentDate = DateTime.Now.AddDays(-3);  // Lấy sản phẩm mới trong 
            var newProducts = await _context.Products.Where(p => p.CreatedDate >= recentDate).ToListAsync();

            return newProducts.Count;
        }

    }
}
