using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Response;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Services.IService
{
    public interface IAuthService
    {
        Task<RegisterResponseDto?> Register(RegisterRequestDto registerRequestDto, string origin);
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto);
        Task<LoginResponseDto> RefreshToken(RefreshTokenDto refreshToken);
        Task<string?> GenerateEmailConfirmationTokenAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> SendPasswordResetEmailAsync(string email, string origin);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<User?> FindByRefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string username);
    }
}
