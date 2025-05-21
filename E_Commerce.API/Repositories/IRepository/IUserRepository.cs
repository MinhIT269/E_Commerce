using E_Commerce.API.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Repositories.IRepository
{
    public interface IUserRepository
    {
        Task<User?> FindByNameAsync(string username);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IdentityResult> AddToRolesAsync(User user, IList<string> roles);
        Task<IdentityResult> UpdateAsync(User user);
        Task<User?> FindByRefreshTokenAsync(string refreshToken);
        Task<User?> FindByIdAsync(string userId);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<User?> FindByEmailAsync(string email);
        Task<IdentityResult?> ResetPasswordAsync(User user, string token, string newPassword);
        IQueryable<User> GetFilteredUsers(string searchQuery, string sortCriteria, bool isDescending);
    }
}
