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
    }
}
