using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApiRequestHelper _apiHelper;
        public OrderController(IConfiguration config, ApiRequestHelper apiRequest)
        {
            _config = config;
            _apiHelper = apiRequest;
        }
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetailOrder(Guid id)
        {
            var order = await _apiHelper.SendGetRequestAsync<OrderResponseDto>($"/api/Orders/OrderDetail/{id.ToString()}");
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> PDF(Guid id)
        {
            var order = await _apiHelper.SendGetRequestAsync<OrderResponseDto>(
                 $"/api/Orders/OrderDetail/{id.ToString()}");
            return new ViewAsPdf(order)
            {
                FileName = "invoice.pdf"
            };
        }
    }
}
