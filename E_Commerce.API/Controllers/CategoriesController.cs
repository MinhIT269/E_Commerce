using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // GET: api/Categories/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryByNameAsync(name);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDto categoryRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _categoryService.IsCategoryNameExistsAsync(categoryRequest.CategoryName!))
            {
                return BadRequest("Tên danh mục đã tồn tại.");
            }

            var success = await _categoryService.CreateCategoryAsync(categoryRequest);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Tạo category thất bại.");

            return Ok(new ApiMessageResponse { Message = "Tạo category thành công." });
        }

        // PUT: api/Categories
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequestDto categoryRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _categoryService.IsCategoryNameExistsAsync(categoryRequest.CategoryName!))
            {
                return BadRequest("Tên danh mục đã tồn tại.");
            }

            var success = await _categoryService.UpdateCategoryAsync(id, categoryRequest);
            if (!success)
                return NotFound("Không tìm thấy category để cập nhật.");

            return Ok("Cập nhật category thành công.");
        }

        // DELETE: api/Categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var productsUsingCategory = await _categoryService.GetProductsByCategoryIdAsync(id);
            if (productsUsingCategory.Any())
            {
                return BadRequest("Danh mục này đang được sử dụng bởi các sản phẩm, không thể xóa!");
            }

            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
                return NotFound("Không tìm thấy category để xoá.");

            return Ok("Xoá category thành công.");
        }

        // GET: api/Categories/GetFilteredCategories?searchQuery=Electronics&page=1&pageSize=10&sortCriteria=name&isDescending=false
        [HttpGet("GetFilteredCategories")]
        public async Task<IActionResult> GetFilteredCategories([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string sortCriteria = "name", [FromQuery] bool isDescending = false)
        {
            var (categories, totalRecords) = await _categoryService.GetFilteredCategoriesAsync(page, pageSize, searchQuery, sortCriteria, isDescending);

            return Ok(categories);
        }

        [HttpGet("TotalPagesCategory")]
        public async Task<IActionResult> GetTotalPagesCategory([FromQuery] string searchQuery = "")
        {
            var totalRecords = await _categoryService.GetTotalCategoriesAsync(searchQuery);
            var totalPages = (int)Math.Ceiling((double)totalRecords / 8); // Điều chỉnh số item trên mỗi trang nếu cần
            return Ok(totalPages);
        }
    }
}
