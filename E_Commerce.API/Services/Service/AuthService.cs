using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Response;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce.API.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }
        public async Task<LoginResponseDto?> Register(RegisterRequestDto registerRequestDto)
        {
            var identityUser = new User
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await _userRepository.CreateAsync(identityUser, registerRequestDto.Password);
            if (!identityResult.Succeeded)
                return null;
            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await _userRepository.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                if (!identityResult.Succeeded)
                    return null;
            }

            return new LoginResponseDto
            {
                IsLogedIn = true
            };
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto)
        {
            var identityUser = await _userRepository.FindByNameAsync(loginRequestDto.Username);
            if (identityUser == null || !await _userRepository.CheckPasswordAsync(identityUser, loginRequestDto.Password))
                return null;

            var roles = await _userRepository.GetRolesAsync(identityUser);

            var jwtToken = _tokenService.CreateJWTToken(identityUser, roles.ToList());
            var refreshToken = _tokenService.CreateRefreshToken();

            identityUser.RefreshToken = refreshToken;
            identityUser.RefreshTokenExpiryTime = System.DateTime.Now.AddDays(7);
            await _userRepository.UpdateAsync(identityUser);

            return new LoginResponseDto
            {
                IsLogedIn = true,
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResponseDto> RefreshToken(RefreshTokenDto refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken.JwtToken);

            var response = new LoginResponseDto();
            if (principal?.Identity?.Name is null)
                return response;
            var user = await _userRepository.FindByNameAsync(principal.Identity.Name);

            if(user is null || user.RefreshToken != refreshToken.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow) 
                return response;
            var roles = await _userRepository.GetRolesAsync(user);

            response.IsLogedIn = true;
            response.JwtToken = _tokenService.CreateJWTToken(user, roles.ToList());
            response.RefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiryTime = System.DateTime.Now.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return response;
        }
    }
}
