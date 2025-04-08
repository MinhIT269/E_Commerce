using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Response;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: /api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _authService.Register(registerRequestDto);
            if (result == null)
            {
                return BadRequest("User registration failed");
            }
            return Ok(result);
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
    }
}
