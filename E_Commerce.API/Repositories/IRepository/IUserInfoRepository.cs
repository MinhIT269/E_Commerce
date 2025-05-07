using E_Commerce.API.Models.Domain;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IUserInfoRepository
    {
        Task<UserInfo?> GetUserByIdAsync(string userId);
        Task AddAsync(UserInfo userInfo);
        Task<UserInfo> CreateAsync(UserInfo userInfo);
        Task<UserInfo?> UpdateAsync(string userId, UserInfo updatedInfo);
    }
}
