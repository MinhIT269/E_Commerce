﻿using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // GET: api/promotion
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(promotions);
        }

        // GET: api/promotion/code/{code}
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var promotion = await _promotionService.GetByCodeAsync(code);
            if (promotion == null) return NotFound();
            return Ok(promotion);
        }

        [HttpGet("GetOne/{code}")]
        public async Task<IActionResult> GetPromotionByCode([FromRoute] Guid code)
        {
            var promotionDTO = await _promotionService.GetPromotion(code);
            if (promotionDTO == null)
            {
                return NotFound();
            }
            return Ok(promotionDTO);
        }

        // GET: api/promotion/filter?searchQuery=...&sortCriteria=...&isDescending=true
        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] string searchQuery, [FromQuery] string sortCriteria, [FromQuery] bool isDescending)
        {
            var filtered = await _promotionService.GetFilteredAsync(searchQuery, sortCriteria, isDescending);
            return Ok(filtered);
        }

        // GET: api/promotion/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _promotionService.GetStatsAsync();
            return Ok(stats);
        }

        // POST: api/promotion
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _promotionService.AddAsync(dto);
            return CreatedAtAction(nameof(GetAll), null);
        }

        // PUT: api/promotion/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PromotionRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _promotionService.UpdateAsync(id, dto);
                return Ok(new ApiMessageResponse { Message = "thành công." }) ;
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/promotion/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _promotionService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetFilteredPromotions")]
        public async Task<IActionResult> GetFilteredPromotions([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "name", [FromQuery] bool isDescending = false)
        {
            var promotions = await _promotionService.GetFilteredPromotionsQuery(page, pageSize, searchQuery, sortCriteria, isDescending);
            return Ok(promotions);
        }

        [HttpGet("TotalPagesPromotions")]
        public async Task<IActionResult> GetTotalPagesPromotion([FromQuery] string searchQuery = "")
        {
            var totalRecords = await _promotionService.GetTotalPromotionAsync(searchQuery);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); // Điều chỉnh số item trên mỗi trang nếu cần
            return Ok(totalPages);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddPromotion(PromotionRequestDto promotionAdd)
        {
            var promotion = await _promotionService.AddPromotion(promotionAdd);
            if (promotion == null)
            {
                return BadRequest("Trung Ma Code");
            }
            return Ok(promotion);
        }
    }
}
