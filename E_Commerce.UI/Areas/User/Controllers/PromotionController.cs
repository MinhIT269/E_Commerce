using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class PromotionController : Controller
    {
        private readonly ApiRequestHelper _apiHelper;
        public PromotionController(ApiRequestHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Validate(string code)
        {
            var result = await _apiHelper.SendGetRequestAsync<PromotionResponseDto>($"/api/promotion/code/{code}");

            if (result == null || result.IsExpired)
            {
                return Json(new { isValid = false });
            }

            return Json(new
            {
                isValid = true,
                value = result.Percentage
            });
        }
    }
}
