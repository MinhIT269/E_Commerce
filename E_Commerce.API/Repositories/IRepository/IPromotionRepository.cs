using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IPromotionRepository
    {
        Task<Promotion?> GetByCodeAsync(string code);
        Task<List<Promotion>> GetAllPromotionAsync();
        Task AddPromotionAsync(Promotion promotion);
        Task UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(Promotion promotion);
        IQueryable<Promotion> GetFilteredPromotionsQuery(string searchQuery, string sortCriteria, bool isDescending);
        Task<object> GetPromotionStatsAsync();
    }
}
