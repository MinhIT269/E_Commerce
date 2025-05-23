using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Services.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IPromotionRepository promotionRepository, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<PromotionResponseDto?> GetByCodeAsync(string code)
        {
            var promotion = await _promotionRepository.GetByCodeAsync(code);
            return promotion == null ? null : _mapper.Map<PromotionResponseDto>(promotion);
        }

        public async Task<PromotionResponseDto> GetPromotion(Guid code)
        {
            var promotion = await _promotionRepository.GetPromotionByIdAsync(code);
            var promotionDto = _mapper.Map<PromotionResponseDto>(promotion);
            return promotionDto;
        }

        public async Task<List<PromotionResponseDto>> GetAllAsync()
        {
            var promotions = await _promotionRepository.GetAllPromotionAsync();
            return _mapper.Map<List<PromotionResponseDto>>(promotions);
        }

        public async Task<List<PromotionResponseDto>> GetFilteredAsync(string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _promotionRepository.GetFilteredPromotionsQuery(searchQuery, sortCriteria, isDescending);
            var filtered = await Task.FromResult(query.ToList()); 
            return _mapper.Map<List<PromotionResponseDto>>(filtered);
        }

        public async Task<object> GetStatsAsync()
        {
            return await _promotionRepository.GetPromotionStatsAsync();
        }

        public async Task AddAsync(PromotionRequestDto promotionDto)
        {
            var promotion = _mapper.Map<Promotion>(promotionDto);
            await _promotionRepository.AddPromotionAsync(promotion);
        }

        public async Task UpdateAsync(Guid id, PromotionRequestDto promotionDto)
        {
            var existingPromotion = (await _promotionRepository.GetAllPromotionAsync())
                                        .FirstOrDefault(p => p.PromotionId == id);

            if (existingPromotion == null)
                throw new Exception("Promotion not found");

            _mapper.Map(promotionDto, existingPromotion);
            await _promotionRepository.UpdatePromotionAsync(existingPromotion);
        }

        public async Task DeleteAsync(Guid id)
        {
            var promotion = (await _promotionRepository.GetAllPromotionAsync())
                            .FirstOrDefault(p => p.PromotionId == id);

            if (promotion != null)
            {
                await _promotionRepository.DeletePromotionAsync(promotion);
            }
            else
            {
                throw new Exception("Promotion not found");
            }
        }

        public async Task<List<PromotionResponseDto>> GetFilteredPromotionsQuery(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _promotionRepository.GetFilteredPromotionsQuery(searchQuery, sortCriteria, isDescending);

            var pagedPromotions = await query.Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ProjectTo<PromotionResponseDto>(_mapper.ConfigurationProvider)
                                             .ToListAsync();
            return pagedPromotions;
        }

        public async Task<int> GetTotalPromotionAsync(string searchQuery)
        {
            var query = _promotionRepository.GetFilteredPromotionsQuery(searchQuery, "name", false);
            return await query.CountAsync();
        }

        public async Task<PromotionResponseDto?> AddPromotion(PromotionRequestDto promotionAdd)
        {
            var promotion = _mapper.Map<Promotion>(promotionAdd);
            var promotions = await GetAllAsync();
            foreach (var i in promotions)
            {
                if (i.Code == promotionAdd.Code)
                {
                    return null;
                }
            }
            var promotionDomain = await _promotionRepository.AddPromotionAsync(promotion);
            var promotionDto = _mapper.Map<PromotionResponseDto>(promotionDomain);
            return promotionDto;
        }
    }
}
