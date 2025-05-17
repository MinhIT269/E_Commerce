using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiRequestHelper _apiHelper;
        public CustomerController(IConfiguration configuration, ApiRequestHelper requestHelper)
        {
            _configuration = configuration;
            _apiHelper = requestHelper;
        }
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        public async Task<IActionResult> Detail([FromQuery] string userName)
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            var user = await _apiHelper.SendGetRequestAsync<UserDto>($"/api/User/{userName}");
            return View(user);
        }
    }
}
