using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "User" });
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Admin")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", new { area = "User" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
