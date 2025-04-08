using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Response;

namespace E_Commerce.API.Services.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> Register(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto);
        Task<LoginResponseDto> RefreshToken(RefreshTokenDto refreshToken);
    }
}
