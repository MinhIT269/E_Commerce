using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IPromotionRepository
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<List<Promotion>> GetAllPromotionAsync();
        Task<Promotion> AddPromotionAsync(Promotion promotion);
        Task<Promotion> UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(Promotion promotion);
        IQueryable<Promotion> GetFilteredPromotionsQuery(string searchQuery, string sortCriteria, bool isDescending);
        Task<object> GetPromotionStatsAsync();
        Task<Promotion?> GetPromotionByIdAsync(Guid code);
    }
}
