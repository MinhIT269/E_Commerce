using E_Commerce.API.Models.Requests;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productsService;
        private readonly IImageService _imageService;
        public ProductsController(IProductService productService, IImageService imageService)
        {
            _productsService = productService;
            _imageService = imageService;
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
        public async Task<IActionResult> GetFilteredProducts([FromQuery] string? filterQuery, [FromQuery] string sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000, [FromQuery] Guid? categoryId = null, [FromQuery] Guid? brandId = null)
        {
            var products = await _productsService.GetProductsFromQuery(filterQuery, sortBy, isAscending, pageNumber, pageSize, categoryId, brandId);
            if (products == null) return NotFound();
            return Ok(products);
        }

        //GET: /api/product/GetProductsFromBrand?brandId=1234-5678-9101-1121&filterQuery=Product1
        [HttpGet("GetProductsFromBrand")]
        public async Task<IActionResult> GetProductsFromBrand([FromQuery] Guid brandId, [FromQuery] string filterQuery)
        {
            var products = await _productsService.GetProductsFromBrand(brandId, filterQuery);
            if (products == null) return NotFound();
            return Ok(products);
        }

        //GET: /api/product/TotalProductsCount?searchQuery=Product1
        [HttpGet("TotalProductsCount")]
        public async Task<IActionResult> GetTotalProductsCount([FromQuery] string? searchQuery, [FromQuery] Guid? categoryId, [FromQuery] Guid? brandId)
        {
            var totalProducts = await _productsService.CountProductAsync(searchQuery, categoryId, brandId);
            return Ok(totalProducts);
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

        //GET: /api/product/GetProductById?productId=1234-5678-9101-1121
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById([FromQuery] Guid productId)
        {
            var product = await _productsService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            try
            {
                await _productsService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("GetProductStats")]
        public async Task<IActionResult> GetProductStats()
        {
            var totalProducts = await _productsService.GetTotalProduct();
            var availableProducts = await _productsService.AvailableProducts();
            var lowStockProducts = await _productsService.GetLowStockProducts();
            var newProducts = await _productsService.GetNewProducts();

            return Ok(new
            {
                TotalProducts = totalProducts,
                AvailableProducts = availableProducts,
                LowStockProducts = lowStockProducts,
                NewProducts = newProducts
            });
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequestDto model, IFormFile ImageUrl, IList<IFormFile> additionalImages)
        {
            try
            {
                model.Description = await _imageService.ProcessDescriptionAndUploadImages(model.Description!, model.ProductId);
                bool isSuccess = await _productsService.AddProductAsync(model, ImageUrl, additionalImages);
                if (isSuccess)
                {
                    return Ok("Product created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create product. Please check the provided data.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Upload-Picture")]
        public async Task<IActionResult> UploadImages(IFormFile files, Guid productId)
        {
            if (files == null)
            {
                return BadRequest("No files were uploaded.");
            }

            var uploadedImageUrls = await _imageService.UploadImageAsync(files, productId);
            return Ok(new { urls = uploadedImageUrls });
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductRequestDto model, IFormFile? mainImage, IList<IFormFile> additionalImages, [FromForm] string? oldImageUrlsJson)
        {
            try
            {
                var oldImageUrls = string.IsNullOrEmpty(oldImageUrlsJson) ? new List<string>()
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(oldImageUrlsJson);

                model.Description = await _imageService.ProcessDescriptionAndUploadImages(model.Description!, model.ProductId);
                bool isSuccess = await _productsService.UpdateProductAsync(model, mainImage, additionalImages, oldImageUrls);
                if (isSuccess)
                {
                    return Ok("Product updated successfully.");
                }
                else
                {
                    return BadRequest("Failed to update product. Please check the provided data.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("upload-temp")]
        public async Task<IActionResult> UploadImages(IFormFile files)
        {
            if (files == null)
            {
                return BadRequest("No files were uploaded.");
            }

            var uploadedImageUrls = await _imageService.UploadImageTempAsync(files, HttpContext);
            return Ok(new { urls = uploadedImageUrls }); // Trả về danh sách đường dẫn hình ảnh dưới dạng JSON
        }
    }
}
