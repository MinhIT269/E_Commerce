using E_Commerce.API.Models.Requests;
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

            var success = await _categoryService.CreateCategoryAsync(categoryRequest);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Tạo category thất bại.");

            return Ok("Tạo category thành công.");
        }

        // PUT: api/Categories
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryRequestDto categoryRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _categoryService.UpdateCategoryAsync(id, categoryRequest);
            if (!success)
                return NotFound("Không tìm thấy category để cập nhật.");

            return Ok("Cập nhật category thành công.");
        }

        // DELETE: api/Categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
                return NotFound("Không tìm thấy category để xoá.");

            return Ok("Xoá category thành công.");
        }
    }
}
