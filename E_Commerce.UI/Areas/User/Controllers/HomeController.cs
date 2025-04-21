using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        private readonly IConfiguration _config;
        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
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
            try
            {
                var client = _httpClientFactory.CreateClient();
                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_baseUrl}/api/Brands")
                };
                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new List<BrandResponseDto>();
                }
                var brandsDto = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<BrandResponseDto>>();
                return brandsDto?.ToList() ?? new List<BrandResponseDto>();
            }
            catch (Exception)
            {
                return new List<BrandResponseDto>();
            }
        }

        public async Task<List<CategoryResponseDto>> GetAllCategories()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_baseUrl}/api/Categories")
                };
                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new List<CategoryResponseDto>();
                }
                var categoryDto = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<CategoryResponseDto>>();
                return categoryDto?.ToList() ?? new List<CategoryResponseDto>();
            }
            catch (Exception)
            {
                return new List<CategoryResponseDto>();
            }
        }

        public async Task<List<ProductResponseDto>> GetProductsByCategory(Guid categoryId, int count)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_baseUrl}/api/Products/GetProductsByCategory?categoryId={categoryId}&count={count}")
                };
                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new List<ProductResponseDto>();
                }
                var productsDto = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<ProductResponseDto>>();
                return productsDto?.ToList() ?? new List<ProductResponseDto>();
            }
            catch (Exception)
            {
                return new List<ProductResponseDto>();
            }
        }

        public async Task<ProductResponseDto> GetProductById(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var httpMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_baseUrl}/api/Products/GetProductById?productId={id}")
                };
                var httpResponse = await client.SendAsync(httpMessage);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return null;
                }
                var productDto = await httpResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
                return productDto;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
