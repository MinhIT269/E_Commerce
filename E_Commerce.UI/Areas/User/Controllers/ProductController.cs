using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiRequestHelper _apiHelper;
        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration config, ApiRequestHelper apiHelper)
        {
            _httpClientFactory = httpClientFactory;
            _apiHelper = apiHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, Guid? categoryId, Guid? brandId)
        {
            var products = await GetProductAsync(search ?? string.Empty, categoryId, brandId);
            return View(products);
        }

        public async Task<IActionResult> FilterProducts(int? count, string? search = "", string sortCriteria = "name", int pageNumber = 1, Guid? categoryId = null, Guid? brandId = null) // gọi bằng Ajax
        {
            count ??= 15;
            var products = await GetProductAsync(search ?? string.Empty, categoryId, brandId, count.Value, sortCriteria, pageNumber);
            return PartialView("_ProductListPartial", products);
        }

        [HttpGet]
        public async Task<JsonResult> GetTotalProductAjax(string search = "", Guid? categoryId = null, Guid? brandId = null)
        {
            var total = await GetTotalProduct(search, categoryId, brandId);
            return Json(total);
        }

        public async Task<List<ProductResponseDto>> GetProductAsync(string? search, Guid? categoryId = null, Guid? brandId = null, int count = 15, string sortCriteria = "name", int pageNumber = 1)
        {
            try
            {
                var sortDirection = sortCriteria.EndsWith("_desc") ? "true" : "false";
                var sortBy = sortCriteria.Contains("price") ? "price" : sortCriteria;
                var relativeUrl = $"/api/Products/GetFilteredProducts?filterQuery={search}&sortBy={sortBy}&isAscending={sortDirection}&pageNumber={pageNumber}&pageSize={count}";

                if (categoryId.HasValue)
                    relativeUrl += $"&categoryId={categoryId.Value}";

                if (brandId.HasValue)
                    relativeUrl += $"&brandId={brandId.Value}";


                var products = await _apiHelper.SendGetRequestAsync<IEnumerable<ProductResponseDto>>(relativeUrl);
                return products?.ToList() ?? new List<ProductResponseDto>();
            }
            catch (Exception)
            {
                return new List<ProductResponseDto>();
            }
        }

        public async Task<int> GetTotalProduct(string search, Guid? categoryId = null, Guid? brandId = null)
        {
            var relativeUrl = $"/api/Products/TotalProductsCount?searchQuery={search}";

            if (categoryId.HasValue)
                relativeUrl += $"&categoryId={categoryId.Value}";

            if (brandId.HasValue)
                relativeUrl += $"&brandId={brandId.Value}";

            var total = await _apiHelper.SendGetRequestAsync<int>(relativeUrl);
            return total;
        }
    }
}
