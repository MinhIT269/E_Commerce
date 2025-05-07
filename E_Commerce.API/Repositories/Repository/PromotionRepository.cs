using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly DataContext _context;
        public PromotionRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code);
        }
    }
}
