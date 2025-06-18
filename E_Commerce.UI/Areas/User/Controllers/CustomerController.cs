using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class CustomerController : Controller
    {
        private readonly IConfiguration _config;
        public CustomerController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Dashboard()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "User" });
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt;

            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return RedirectToAction("Login", "Auth", new { area = "User" });
            }

            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth", new { area = "User" });
            }

            // Nếu hợp lệ thì render View
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Token is missing." });

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt;

            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return BadRequest(new { message = "Invalid token." });
            }

            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "User ID not found in token." });

            return Ok(new
            {
                id = userId,
                name = name,
                email = email,
                role = role
            });
        }
    }
}
