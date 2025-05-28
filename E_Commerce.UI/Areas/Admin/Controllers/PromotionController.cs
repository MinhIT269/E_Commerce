using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : AdminBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ApiRequestHelper _apiRequestHelper;
        public PromotionController(IConfiguration configuration, ApiRequestHelper apiRequestHelper)
        {
            _configuration = configuration;
            _apiRequestHelper = apiRequestHelper;
        }
        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PromotionRequestDto promotion)
        {
            var result = await _apiRequestHelper.SendPostRequestAsync<PromotionResponseDto>("/api/Promotion/Add", promotion);
            if (result != null)
            {
                return RedirectToAction("Index");
            }
            // Nếu có lỗi khi tạo, thêm lỗi vào ModelState và trả về trang tạo
            ModelState.AddModelError(string.Empty, "Mã khuyễn mãi đã tồn tại!");
            return View(promotion); 
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var promotion = await _apiRequestHelper.SendGetRequestAsync<PromotionResponseDto>($"/api/Promotion/GetOne/{id.ToString()}");
            return View(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] PromotionResponseDto promotionDto)
        {
            var id = promotionDto.PromotionId; // Lấy id từ DTO
            var url = $"/api/Promotion/{id}";

            var result = await _apiRequestHelper.SendPutRequestAsync<ApiMessageResponse>(url, promotionDto);

            if (result != null)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật phiếu giảm giá");
            return View(promotionDto);
        }
    }
}
