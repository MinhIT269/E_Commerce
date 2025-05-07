using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IUserInfoService
    {
        Task<UserInfoDto?> GetByUserIdAsync(string userId);
        Task<UserInfoDto> CreateAsync(UserInfoDto userInfo);
        Task<UserInfoDto?> UpdateAsync(string userId, UserInfoDto userInfo);
    }
}
