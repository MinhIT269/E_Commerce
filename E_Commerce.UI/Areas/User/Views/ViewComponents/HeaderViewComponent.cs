using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Views.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        public HeaderViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Header");
        }
    }
}