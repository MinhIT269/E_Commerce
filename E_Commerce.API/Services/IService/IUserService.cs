using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IUserService
    {
        Task<int> GetTotalUserAsync(string searchQuery);
        Task<List<UserAdminDto>> GetFilteredUsers(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending);
        Task<UserDto> GetUser(string username);
    }
}
