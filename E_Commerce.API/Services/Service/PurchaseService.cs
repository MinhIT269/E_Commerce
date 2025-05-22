using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Services.Service
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper mapper;
        public PurchaseService(IPurchaseRepository purchaseRepository, IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            this.mapper = mapper;
        }
        public async Task<List<OrderDto>> GetFilteredOrders(int page, int pageSize, string searchQuery, string sortCriteria)
        {
            var query = _purchaseRepository.GetFilteredOrdersPurchase(searchQuery, sortCriteria);

            var pagedOrders = await query.Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                                             .ToListAsync();
            return pagedOrders!;
        }

        public async Task<int> GetTotalOrdersAsync(string searchQuery, string sortCriteria)
        {
            var query = _purchaseRepository.GetFilteredOrdersPurchase(searchQuery, sortCriteria);
            return await query.CountAsync();
        }
        public async Task<object> GetPaymentMethodStatisticsAsync()
        {
            return await _purchaseRepository.GetPaymentMethodStatisticsAsync();
        }
    }
}
