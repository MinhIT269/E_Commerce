using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class AuthController : Controller
    {
        private readonly ApiRequestHelper _apiRequestHelper;
        public AuthController(ApiRequestHelper apiRequestHelper)
        {
            _apiRequestHelper = apiRequestHelper;
        }
        public IActionResult Login()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var loginResponse = await _apiRequestHelper.SendPostRequestAsync<LoginResponseDto>("/api/Auth/Login", model);

            if (loginResponse != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                };

                var refreshOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("JwtToken", loginResponse.JwtToken!, cookieOptions);
                Response.Cookies.Append("RefreshToken", loginResponse.RefreshToken!, refreshOptions);

                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            ViewBag.Message = "Đăng nhập thất bại: Sai tài khoản hoặc mật khẩu.";
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            var registerResponse = await _apiRequestHelper.SendPostRequestAsync<RegisterResponseDto>("/api/Auth/Register", model);

            if (registerResponse != null && registerResponse.IsSuccessful)
            {
                ViewBag.Message = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản.";
                return View();
            }

            ViewBag.Message = "Đăng nhập thất bại: Sai tài khoản hoặc mật khẩu.";
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(ForgotPasswordRequestDto forgotPassword)
        {
            var response = await _apiRequestHelper.SendPostRequestAsync<ApiMessageResponse>("/api/Auth/ForgotPassword", forgotPassword);
            if (response != null)
            {
                ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
            }
            else
            {
                ViewBag.ErrorMessage = "Không thể gửi email. Vui lòng thử lại.";
            }
            return View("ResetPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var response = await _apiRequestHelper.SendPostRequestAsync<ApiMessageResponse>("/api/Auth/ResetPassword", model);

            if (response != null)
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
    }
}
