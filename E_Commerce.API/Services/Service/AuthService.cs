using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Response;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;


namespace E_Commerce.API.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public AuthService(IUserRepository userRepository, ITokenService tokenService, IEmailService emailService)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<RegisterResponseDto?> Register(RegisterRequestDto registerRequestDto, string origin)
        {
            var identityUser = new User
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await _userRepository.CreateAsync(identityUser, registerRequestDto.Password);
            if (!identityResult.Succeeded)
            {
                return new RegisterResponseDto
                {
                    IsSuccessful = false
                };
            }

            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await _userRepository.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                if (!identityResult.Succeeded)
                {
                    return new RegisterResponseDto
                    {
                        IsSuccessful = false,
                    };
                }
            }

            // Tạo token xác nhận email
            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(identityUser);

            var confirmationLink = $"{origin}/api/Auth/ConfirmEmail?userId={identityUser.Id}&token={Uri.EscapeDataString(token)}";
            var emailBody = $@"
                     <h2>Welcome, {registerRequestDto.Username}</h2>
                     <p>Please confirm your email by clicking the link below:</p>
                     <a href='{confirmationLink}'>Confirm Email</a>";

            await _emailService.SendEmailAsync(registerRequestDto.Username, "Confirm your email", emailBody);

            return new RegisterResponseDto
            {
                IsSuccessful = true,
                UserId = identityUser.Id
            };
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequestDto)
        {
            var identityUser = await _userRepository.FindByNameAsync(loginRequestDto.Username);
            if (identityUser == null || !await _userRepository.CheckPasswordAsync(identityUser, loginRequestDto.Password))
                return null;

            if (!identityUser.EmailConfirmed)
            {
                return new LoginResponseDto
                {
                    IsLogedIn = false,
                    ErrorMessage = "Email not confirmed. Please check your email to confirm."
                };
            }

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

            if (user is null || user.RefreshToken != refreshToken.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return response;
            var roles = await _userRepository.GetRolesAsync(user);

            response.IsLogedIn = true;
            response.JwtToken = _tokenService.CreateJWTToken(user, roles.ToList());
            response.RefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiryTime = System.DateTime.Now.AddDays(7);
            await _userRepository.UpdateAsync(user);
            //response.RefreshToken = user.RefreshToken;

            return response;
        }

        public async Task<string?> GenerateEmailConfirmationTokenAsync(string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null) return null;

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userRepository.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
                return false;

            var token = await _userRepository.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{origin}/user/auth/resetpassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            var emailBody = $@"
                     <p>Click the link below to reset your password:</p>
                     <a href='{resetLink}'>Reset Password</a>";

            await _emailService.SendEmailAsync(email, "Reset Your Password", emailBody);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            var result = await _userRepository.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            return result!.Succeeded;
        }

        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {
            return await _userRepository.FindByRefreshTokenAsync(refreshToken);
        }

        public async Task LogoutAsync(string username)
        {
            var user = await _userRepository.FindByNameAsync(username);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
