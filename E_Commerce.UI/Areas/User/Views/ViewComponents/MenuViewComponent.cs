using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        public MenuViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await GetAllBrand();
            var categories = await GetAllCategories();

            var viewModel = new MenuViewModel
            {
                Brands = brands,
                Categories = categories
            };

            return View(viewModel);  
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

        private async Task<List<CategoryResponseDto>> GetAllCategories()
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
    }
}
