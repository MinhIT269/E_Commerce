using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IPromotionService
    {
        Task<PromotionResponseDto?> GetByCodeAsync(string code);
        Task<List<PromotionResponseDto>> GetAllAsync();
        Task<List<PromotionResponseDto>> GetFilteredAsync(string searchQuery, string sortCriteria, bool isDescending);
        Task<object> GetStatsAsync();
        Task AddAsync(PromotionRequestDto promotionDto);
        Task UpdateAsync(Guid id, PromotionRequestDto promotionDto);
        Task DeleteAsync(Guid id);
    }
}
