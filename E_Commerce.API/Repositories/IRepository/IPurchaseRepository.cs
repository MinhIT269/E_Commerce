using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IPurchaseRepository
    {
        IQueryable<Order> GetFilteredOrdersPurchase(string searchQuery, string sortCriteria);
        Task<object> GetPaymentMethodStatisticsAsync();
    }
}
