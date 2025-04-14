using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productsService;
        public ProductsController(IProductService productService)
        {
            _productsService = productService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsService.GetAllProducts();
            if (products == null) return NotFound();
            return Ok(products);
        }

        //GET: /api/product?filterQuery=Product1&sortBy=Price&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet("GetFilteredProducts")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var products = await _productsService.GetProductsFromQuery(filterQuery, sortBy, isAscending, pageNumber, pageSize);
            if(products == null) return NotFound();
            return Ok(products);
        }

        //GET: /api/product/GetProductsFromBrand?brandId=1234-5678-9101-1121&filterQuery=Product1
        [HttpGet("GetProductsFromBrand")]
        public async Task<IActionResult> GetProductsFromBrand ([FromQuery] Guid brandId, [FromQuery] string filterQuery)
        {
            var products = await _productsService.GetProductsFromBrand(brandId, filterQuery);
            if (products == null) return NotFound();
            return Ok(products);
        }

        //GET: /api/product/TotalPagesProduct?searchQuery=Product1
        [HttpGet("TotalPagesProduct")]
        public async Task<IActionResult> GetTotalPagesForAllProducts([FromQuery] string? searchQuery)
        {
            var totalPages = await _productsService.CountProductAsync(searchQuery ?? string.Empty);
            return Ok(totalPages);
        }

        //GET: /api/product/GetProductsByCategory?categoryId=1234-5678-9101-1121&count=20
        [HttpGet("GetProductsByCategory")]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] Guid categoryId, [FromQuery] int count)
        {
            var products = await _productsService.GetProductsByCategoryAsync(categoryId, count);
            if (products == null || !products.Any())
                return NotFound();

            return Ok(products);
        }

    }
}
