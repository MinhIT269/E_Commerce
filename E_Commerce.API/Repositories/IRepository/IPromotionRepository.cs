using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IPromotionRepository
    {
        Task<Promotion?> GetByCodeAsync(string code);
    }
}
