using E_Commerce.API.Models.Responses;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _service;
        public UserInfoController(IUserInfoService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var result = await _service.GetByUserIdAsync(userId);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserInfoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, UserInfoDto dto)
        {
            var updated = await _service.UpdateAsync(userId, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
