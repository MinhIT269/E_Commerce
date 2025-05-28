using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PurchaseController : AdminBaseController
    {
        private readonly IConfiguration _config;
        public PurchaseController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }
    }
}
