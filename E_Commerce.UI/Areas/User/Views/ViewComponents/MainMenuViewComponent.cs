using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Views.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly ApiRequestHelper _apiHelper;
        public MainMenuViewComponent(ApiRequestHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _apiHelper.SendGetRequestAsync<List<CategoryResponseDto>>("/api/categories");
            var brands = await _apiHelper.SendGetRequestAsync<List<BrandResponseDto>>("/api/brands");
            var viewModel = new MenuViewModel
            {
                Categories = categories ?? new List<CategoryResponseDto>(),
                Brands = brands ?? new List<BrandResponseDto>()
            };
            return View(viewModel);
        }
    }
}
