using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {

            _userService = userService;
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserDetails(string username)
        {
            var user = await _userService.GetUser(username);

            return Ok(user);
        }

        [HttpGet("GetFilteredUsers")]
        public async Task<IActionResult> GetFilteredUsers([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "name", [FromQuery] bool isDescending = false)
        {
            var users = await _userService.GetFilteredUsers(page, pageSize, searchQuery, sortCriteria, isDescending);
            return Ok(users);
        }

        [HttpGet("TotalPagesUsers")]
        public async Task<IActionResult> GetTotalUsersPromotion([FromQuery] string searchQuery = "")
        {
            var totalRecords = await _userService.GetTotalUserAsync(searchQuery);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); // Điều chỉnh số item trên mỗi trang nếu cần
            return Ok(totalPages);
        }

        [HttpGet("TotalUsers")]
        public async Task<IActionResult> TotalUsers([FromQuery] string searchQuery = "")
        {
            try
            {
                var totalRecords = await _userService.GetTotalUserAsync(searchQuery);
                return Ok(new { totalUsers = totalRecords });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
    }
}
