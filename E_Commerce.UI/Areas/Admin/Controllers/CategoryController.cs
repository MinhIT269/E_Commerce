using Microsoft.AspNetCore.Mvc;
using E_Commerce.UI.Models.Responses;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Helpers;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiRequestHelper _apiRequestHelper;
        public CategoryController(IConfiguration config, ApiRequestHelper apiRequestHelper)
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryRequestDto category)
        {
            var result = await _apiRequestHelper.SendPostRequestAsync<ApiMessageResponse>("/api/Categories", category);

            if (result != null)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Lỗi khi tạo danh mục");
            return RedirectToAction("Error", "");
        }
        public IActionResult Edit(Guid id)
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
        //    var category = await _apiHelper.GetDataFromApi<CategoryResponseDto>($"/api/Category/{id.ToString()}");
            return View();
        }
    }
}
