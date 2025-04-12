using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        // GET: api/Brands/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound();
            return Ok(brand);
        }

        // GET: api/Brands/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetBrandByName(string name)
        {
            var brand = await _brandService.GetBrandByNameAsync(name);
            if (brand == null)
                return NotFound();
            return Ok(brand);
        }

        // POST: api/Brands
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] BrandRequestDto brandRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var success = await _brandService.CreateBrandAsync(brandRequest);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Tạo brand thất bại.");
            return Ok("Tạo brand thành công.");
        }

        // PUT: api/Brands/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromBody] BrandRequestDto brandRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var success = await _brandService.UpdateBrandAsync(id, brandRequest);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Cập nhật brand thất bại.");
            return Ok("Cập nhật brand thành công.");
        }

        // DELETE: api/Brands/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var success = await _brandService.DeleteBrandAsync(id);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa brand thất bại.");
            return Ok("Xóa brand thành công.");
        }
    }
}
