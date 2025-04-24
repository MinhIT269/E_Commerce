using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await _authService.Register(registerRequestDto, origin);
            if (result == null)
            {
                return BadRequest("User registration failed");
            }

            return Ok(result);
        }

        // GET: /api/Auth/ConfirmEmail
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var success = await _authService.ConfirmEmailAsync(userId, token);
            if (!success)
            {
                return BadRequest("Invalid or expired email confirmation link.");
            }
            var loginUrl = $"{_configuration["Frontend:BaseUrl"]}{_configuration["Frontend:LoginPath"]}";
            return Redirect(loginUrl);
        }

        // POST: /api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _authService.Login(loginRequestDto);
            if (result == null)
                return BadRequest("Registration failed");

            return Ok(result);
        }

        // POST: /api/Auth/RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshToken(refreshTokenDto);
            if (result.IsLogedIn)
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        // POST: /api/Auth/ForgotPassword
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var origin = $"{_configuration["Frontend:BaseUrl"]}";
            var success = await _authService.SendPasswordResetEmailAsync(request.Email, origin);

            if (!success)
                return BadRequest("Failed to send password reset email.");

            return Ok(new { message = "Password reset link has been sent to your email." });
        }

        // POST: /api/Auth/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result)
                return BadRequest("Failed to reset password.");

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
