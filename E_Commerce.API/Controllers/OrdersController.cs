using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    { 
        private readonly IOrderService _orderService;
        private readonly IConfiguration _config;
        public OrdersController(IOrderService orderService, IConfiguration config)
        {
            _orderService = orderService;
            _config = config;
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderRequestDto request)
        {
            try
            {
                var paymentUrl = await _orderService.CreateOrderAndPaymentAsync(request, HttpContext);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("PaymentBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = await _orderService.HandlePaymentCallbackAsync(Request.Query);
            var frontendBaseUrl = _config["Frontend:BaseUrl"];
            var redirectUrl = $"{frontendBaseUrl}/User/CheckOut/Result?" +
                  $"message={WebUtility.UrlEncode(response.Message)}" +
                  $"&transactionId={response.TransactionId}" +
                  $"&amount={response.Amount}" +
                  $"&statusText={WebUtility.UrlEncode(response.StatusText)}" +
                  $"&statusClass={response.StatusClass}";
            return Redirect(redirectUrl);
        }

        [HttpGet("GetFilterOrdered")]
        public async Task<IActionResult> GetFilteredCategories([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "name", [FromQuery] bool isDescending = false)
        {
            var orders = await _orderService.GetFilteredOrders(page, pageSize, searchQuery, sortCriteria, isDescending);

            return Ok(orders);
        }

        [HttpGet("TotalPagesOrdered")]
        public async Task<IActionResult> GetTotalPagesCategory([FromQuery] string searchQuery = "")
        {
            var totalRecords = await _orderService.GetTotalOrdersAsync(searchQuery);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); 
            return Ok(totalPages);
        }

        [HttpGet("TotalOrders")]
        public async Task<IActionResult> TotalOrders()
        {
            var totalOrders = await _orderService.TotalOrders();
            return Ok(totalOrders);
        }

        [HttpGet("GetOrdersStats")]
        public async Task<IActionResult> GetProductStats()
        {
            var totalOrders = await _orderService.TotalOrders();
            var totalOrdersPending = await _orderService.TotalOrdersPending();
            var totalOrdersSuccess = await _orderService.TotalOrdersSuccess();
            var totalOrdersCancel = await _orderService.TotalOrdersCancel();
            var totalAmount = await _orderService.GetTotalAmountOfCompletedOrdersAsync();
            return Ok(new
            {
                TotalOrders = totalOrders,
                OrdersPending = totalOrdersPending,
                OrderSuccess = totalOrdersSuccess,
                OrderCancel = totalOrdersCancel,
                TotalAmount = totalAmount
            });
        }

        [HttpGet("OrderDetail/{id:guid}")]
        public async Task<IActionResult> GetOrderDetail([FromRoute] Guid id)
        {
            var orderDetailDTO = await _orderService.GetOrderDetails(id);
            if (orderDetailDTO == null)
            {
                return NotFound();
            }
            return Ok(orderDetailDTO);
        }

        [HttpGet("AllOrder")]
        public async Task<IActionResult> GetAllOrder([FromQuery] string id, [FromQuery] string? searchQuery, [FromQuery] int page = 1, int pageSize = 5)
        {
            try
            {
                var ordersDTO = await _orderService.GetAllOrders(id, searchQuery, page, pageSize);

                // Kiểm tra nếu không có đơn hàng nào được tìm thấy
                if (ordersDTO == null || !ordersDTO.Any())
                {
                    return NotFound(new { Message = "No orders found for the given user ID and search query." });
                }

                return Ok(ordersDTO);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không mong muốn
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Details = ex.Message });
            }
        }

        [HttpGet("TotalPagesOrdered_Detail")]
        public async Task<IActionResult> GetTotalPagesCategory_Detail([FromQuery] string id, [FromQuery] string searchQuery = "")
        {
            var totalPages = await _orderService.CountAllOrdersAsync(id, searchQuery);
            return Ok(totalPages);
        }


        [HttpGet("GetOrdersStats_UserDetail")]
        public async Task<IActionResult> GetOrdersStats_UserDetail([FromQuery] string id)
        {
            var totalOrders = await _orderService.TotalOrdersByUser(id);
            var totalOrdersPending = await _orderService.TotalOrdersPendingByUser(id);
            var totalOrdersSuccess = await _orderService.TotalOrdersSuccessByUser(id);
            var sumOrder = await _orderService.SumCompletedOrdersAmountByUser(id);
            return Ok(new
            {
                TotalOrders = totalOrders,
                OrdersPending = totalOrdersPending,
                OrderSuccess = totalOrdersSuccess,
                SumOrder = sumOrder
            });
        }
    }
}
