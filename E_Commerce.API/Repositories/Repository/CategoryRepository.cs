using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == name);
        }
        public async Task<bool> CategoryExistsAsync(Guid id)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == id);
        }
        public async Task<bool> CategoryExistsByNameAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryName == name);
        }
        public async Task<bool> CreateCategoryAsync(Category category)
        {
            if (category == null) return false;
            _context.Categories.Add(category);
            return await SaveChangesAsync();
        }
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.CategoryId);
            if (existing == null) return false;

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.ParentCategoryId = category.ParentCategoryId;
            return await SaveChangesAsync();
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            return await SaveChangesAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public IQueryable<Category> GetFilteredCategoriesQuery(string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _context.Categories
                                    .Include(c => c.ProductCategories)!
                                    .ThenInclude(p => p.Product)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => EF.Functions.Collate(c.CategoryName!, "SQL_Latin1_General_CP1_CI_AI").Contains(searchQuery));
            }

            query = sortCriteria switch
            {
                "name" => isDescending ? query.OrderByDescending(c => c.CategoryName) : query.OrderBy(c => c.CategoryName),
                "productCount" => isDescending ? query.OrderByDescending(c => c.ProductCategories!.Sum(pc => pc.Product!.Quantity)) : query.OrderBy(c => c.ProductCategories!.Sum(pc => pc.Product!.Quantity)),
                _ => query
            };

            return query;
        }
        public async Task<List<Product>> GetProductByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products.Where(p => p.ProductCategories!.Any(pc => pc.CategoryId == categoryId)).ToListAsync();
        }

        public async Task<bool> IsCategoryNameExistsAsync(string categoryName)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryName == categoryName);
        }
    }
}
