using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet("AllPaymentOrders")]
        public async Task<IActionResult> GetFilteredPromotions([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "All")
        {
            try
            {
                // Gọi service để lấy danh sách đơn hàng
                var ordersDTO = await _purchaseService.GetFilteredOrders(page, pageSize, searchQuery, sortCriteria);

                // Kiểm tra nếu không có đơn hàng nào được tìm thấy
                if (ordersDTO == null || !ordersDTO.Any())
                {
                    return NotFound(new { Message = "No orders found" });
                }

                // Trả về danh sách đơn hàng
                return Ok(ordersDTO);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không mong muốn
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }

        [HttpGet("TotalPagesOrdered")]
        public async Task<IActionResult> GetTotalPagesCategory([FromQuery] string searchQuery = "", [FromQuery] string sortCriteria = "All")
        {
            var totalRecords = await _purchaseService.GetTotalOrdersAsync(searchQuery, sortCriteria);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); // Điều chỉnh số item trên mỗi trang nếu cần
            return Ok(totalPages);
        }

        [HttpGet("GetStatsPayMent")]
        public async Task<IActionResult> GetPromotionStats()
        {
            try
            {
                var stats = await _purchaseService.GetPaymentMethodStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
