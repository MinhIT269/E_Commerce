using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class AuthController : Controller
    {
        private readonly ApiRequestHelper _api;
        public AuthController(ApiRequestHelper apiRequestHelper) => _api = apiRequestHelper;

        public IActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var res = await _api.SendPostRequestAsync<LoginResponseDto>("/api/Auth/Login", model);
            if (res == null)
            {
                ViewBag.Message = "Đăng nhập thất bại: Sai tài khoản hoặc mật khẩu.";
                return View();
            }

            var jwtOptions = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddHours(1) };
            var refreshOptions = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTimeOffset.UtcNow.AddDays(7) };

            Response.Cookies.Append("JwtToken", res.JwtToken!, jwtOptions);
            Response.Cookies.Append("RefreshToken", res.RefreshToken!, refreshOptions);

            // Giải mã token để đọc role
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(res.JwtToken!);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Điều hướng theo role
            if (role == "Admin")
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            var res = await _api.SendPostRequestAsync<RegisterResponseDto>("/api/Auth/Register", model);
            ViewBag.Message = res != null && res.IsSuccessful
                ? "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản."
                : "Đăng ký thất bại.";
            return View();
        }

        public IActionResult ResetPassword() => View();

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(ForgotPasswordRequestDto model)
        {
            var res = await _api.SendPostRequestAsync<ApiMessageResponse>("/api/Auth/ForgotPassword", model);
            if (res != null)
                ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
            else
                ViewBag.ErrorMessage = "Không thể gửi email. Vui lòng thử lại.";

            return View("ResetPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var res = await _api.SendPostRequestAsync<ApiMessageResponse>("/api/Auth/ResetPassword", model);
            if (res != null)
            {
                TempData["Message"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login");
            }

            ViewBag.ErrorMessage = "Đặt lại mật khẩu thất bại. Hãy kiểm tra token hoặc thông tin nhập.";
            return View("ResetPassword");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("JwtToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
