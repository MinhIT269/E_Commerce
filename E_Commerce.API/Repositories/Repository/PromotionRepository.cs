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
            return _context.Promotions
                .AsEnumerable() 
                .FirstOrDefault(p => p.Code!.Equals(code, StringComparison.Ordinal));
        }

        public async Task<Promotion?> GetPromotionByIdAsync(Guid code)
        {
            var promotion = await _context.Promotions
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(x => x.PromotionId == code);
            return promotion;
        }

        public async Task<List<Promotion>> GetAllPromotionAsync()
        {
            var promotions = await _context.Promotions.AsNoTracking().ToListAsync();
            return promotions;
        }

        public async Task<Promotion> AddPromotionAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task DeletePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }
        public IQueryable<Promotion> GetFilteredPromotionsQuery(string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _context.Promotions.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => EF.Functions.Collate(c.Code!, "SQL_Latin1_General_CP1_CI_AI").Contains(searchQuery));
            }

            query = sortCriteria switch
            {
                "name" => isDescending ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
                "endDate" => isDescending ? query.OrderByDescending(c => c.EndDate) : query.OrderBy(c => c.EndDate),
                "startDate" => isDescending ? query.OrderBy(c => c.StartDate) : query.OrderByDescending(c => c.StartDate),
                _ => query
            };

            return query;
        }

        public async Task<object> GetPromotionStatsAsync()
        {
            // Tổng số Promotion
            var totalPromotions = await _context.Promotions.CountAsync();

            // Số chương trình khuyến mãi còn hiệu lực
            var activePromotions = await _context.Promotions
                                                   .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
                                                   .CountAsync();

            // Số chương trình khuyến mãi đã hết hạn
            var expiredPromotions = await _context.Promotions
                                                    .Where(p => p.EndDate < DateTime.Now)
                                                    .CountAsync();

            // Số chương trình khuyến mãi chưa bắt đầu
            var upcomingPromotions = await _context.Promotions
                                                     .Where(p => p.StartDate > DateTime.Now)
                                                     .CountAsync();

            return new
            {
                TotalPromotions = totalPromotions,
                ActivePromotions = activePromotions,
                ExpiredPromotions = expiredPromotions,
                UpcomingPromotions = upcomingPromotions
            };
        }
    }
}
