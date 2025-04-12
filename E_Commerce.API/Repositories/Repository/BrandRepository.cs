using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly DataContext _context;
        public BrandRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllBrandsAsync() 
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetBrandByIdAsync(Guid id) 
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand?> GetBrandByNameAsync(string name) 
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.BrandName == name);
        }

        public async Task<bool> BrandExistsAsync(Guid id)
        {
            return await _context.Brands.AnyAsync(b => b.BrandId == id);
        }

        public async Task<bool> BrandExistsByNameAsync(string name)
        {
            return await  _context.Brands.AnyAsync(b => b.BrandName == name);
        }

        public async Task<bool> CreateBrandAsync(Brand brand)
        {
            if (brand == null) return false;
            await _context.Brands.AddAsync(brand);
            return await SaveChangesAsync();
        }

        public async Task<bool> UpdateBrandAsync(Brand brand) 
        {
            var existing = await _context.Brands.FindAsync(brand.BrandId);
            if (existing == null) return false;

            _context.Brands.Update(brand);
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteBrandAsync(Guid id) 
        {
            var existing = await _context.Brands.FindAsync(id);
            if (existing == null) return false;

            _context.Brands.Remove(existing);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
