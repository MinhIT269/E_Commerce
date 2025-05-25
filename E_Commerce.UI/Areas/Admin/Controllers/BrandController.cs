using Microsoft.AspNetCore.Mvc;
using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using E_Commerce.UI.Models.Requests;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiRequestHelper _apiRequestHelper;
        public BrandController(IConfiguration config, ApiRequestHelper apiRequestHelper)
        {
            _config = config;
            _apiRequestHelper = apiRequestHelper;
        }
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BrandRequestDto request)
        {
            var result = await _apiRequestHelper.SendPostRequestAsync<ApiMessageResponse>("/api/Brands", request);
         
            if (result != null)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Lỗi khi tạo danh mục");
            return View();
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            var brand = await _apiRequestHelper.SendGetRequestAsync<BrandResponseDto>($"/api/Brands/{id.ToString()}");
            return View(brand);
        }
    }
}
