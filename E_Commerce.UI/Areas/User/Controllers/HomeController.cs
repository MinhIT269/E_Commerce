using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ApiRequestHelper _apiHelper;
        private readonly IConfiguration _config;
        public HomeController(ApiRequestHelper apiRequestHelper, IConfiguration config)
        {
            _apiHelper = apiRequestHelper;
            _config = config;
        }
        public async Task<IActionResult> Index()
        {
            var laptopId = Guid.Parse(_config["CategorySettings:Laptop"]!);
            var pcId = Guid.Parse(_config["CategorySettings:Pc"]!);
            var headphoneId = Guid.Parse(_config["CategorySettings:Headphone"]!);


            var brandsDto = await GetAllBrand();
            var categoriesDto = await GetAllCategories();

            var ViewModel = new HomeViewModel()
            {
                Brands = brandsDto,
                Categories = categoriesDto,
                CategoryBlocks = new List<CategoryProductDto>()
                {
                    new CategoryProductDto { CategoryName = "Laptop", Products = await GetProductsByCategory(laptopId, 15) },
                    new CategoryProductDto { CategoryName = "PC", Products = await GetProductsByCategory(pcId, 15) },
                    new CategoryProductDto { CategoryName = "Headphone", Products = await GetProductsByCategory(headphoneId, 15) }
                }
            };
            return View(ViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var product = await GetProductById(id);
            return View(product);
        }

        public async Task<List<BrandResponseDto>> GetAllBrand()
        {
            return (await _apiHelper.SendGetRequestAsync<IEnumerable<BrandResponseDto>>("/api/Brands"))?.ToList() ?? new List<BrandResponseDto>();
        }

        public async Task<List<CategoryResponseDto>> GetAllCategories()
        {
            return (await _apiHelper.SendGetRequestAsync<IEnumerable<CategoryResponseDto>>("/api/Categories"))?.ToList() ?? new List<CategoryResponseDto>();
        }

        public async Task<List<ProductResponseDto>> GetProductsByCategory(Guid categoryId, int count)
        {
            var url = $"/api/Products/GetProductsByCategory?categoryId={categoryId}&count={count}";
            return (await _apiHelper.SendGetRequestAsync<IEnumerable<ProductResponseDto>>(url))?.ToList() ?? new List<ProductResponseDto>();
        }

        public async Task<ProductResponseDto?> GetProductById(Guid id)
        {
            return await _apiHelper.SendGetRequestAsync<ProductResponseDto>($"/api/Products/GetProductById?productId={id}");
        }
    }
}
