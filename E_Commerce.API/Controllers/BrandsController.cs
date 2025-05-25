using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
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
            if (await _brandService.IsBrandNameExists(brandRequest.BrandName!))
            {
                return BadRequest("Tên thương hiệu đã tồn tại.");
            }
            var success = await _brandService.CreateBrandAsync(brandRequest);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Tạo brand thất bại.");
            return Ok(new ApiMessageResponse { Message = "Tạo brand thành công."});
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
            var productsUsingBrand = await _brandService.HasProductsByBrandIdAsync(id);
            if (productsUsingBrand)
            {
                return BadRequest("Thương hiệu này đang được sử dụng bởi các sản phẩm, không thể xóa!");
            }
            var success = await _brandService.DeleteBrandAsync(id);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Xóa brand thất bại.");
            return Ok("Xóa brand thành công.");
        }

        // GET: api/Brands/filtered
        [HttpGet("GetFilteredBrands")]
        public async Task<IActionResult> GetFilteredBrands([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "name", [FromQuery] bool isDescending = false)
        {
            var (brands, totalRecords) = await _brandService.GetFilteredCategoriesAsync(page, pageSize, searchQuery, sortCriteria, isDescending);
            return Ok(brands);
        }

        // GET: api/Brands/TotalPagesBrands?searchQuery=Electronics
        [HttpGet("TotalPagesBrands")]
        public async Task<IActionResult> GetTotalPagesCategory([FromQuery] string searchQuery = "")
        {
            var totalRecords = await _brandService.GetTotalBrandsAsync(searchQuery);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); // Điều chỉnh số item trên mỗi trang nếu cần
            return Ok(totalPages);
        }
    }
}
