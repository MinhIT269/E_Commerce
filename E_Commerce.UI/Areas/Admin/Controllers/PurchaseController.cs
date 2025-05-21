using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PurchaseController : Controller
    {
        private readonly IConfiguration _config;
        public PurchaseController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
