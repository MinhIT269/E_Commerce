using E_Commerce.UI.Helpers;
using E_Commerce.UI.Models.Requests;
using E_Commerce.UI.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : AdminBaseController
    {
        private readonly IConfiguration _config;
        private readonly ApiRequestHelper _apiHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductController(IConfiguration config, ApiRequestHelper apiHelper, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _apiHelper = apiHelper;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            var categories = await _apiHelper.SendGetRequestAsync<IEnumerable<CategoryResponseDto>>("/api/Categories");
            var brands = await _apiHelper.SendGetRequestAsync<IEnumerable<BrandResponseDto>>("/api/Brands");

            var model = new CreateProductViewModel
            {
                Categories = categories ?? [],
                Brands = brands ?? []
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductRequestDto model, IFormFile ImageUrl, IList<IFormFile> additionalImages)
        {
            var Url = _config["ApiSettings:BaseUrl"];
            model.CreatedDate = DateTime.Now.Date;
            model.ProductId = Guid.NewGuid();

            var content = new MultipartFormDataContent(); // Tạo MultipartFormDataContent để gửi tới API

            // Thêm các trường vào content
            content.Add(new StringContent(model.ProductId.ToString()), "ProductId");
            content.Add(new StringContent(model.Name!), "Name");
            content.Add(new StringContent(model.Description!), "Description");
            content.Add(new StringContent(model.MetaDescription!), "MetaDescription");
            content.Add(new StringContent(model.CreatedDate.ToString()), "CreatedDate");
            content.Add(new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price");
            content.Add(new StringContent(model.PromotionPrice?.ToString(CultureInfo.InvariantCulture)!), "PromotionPrice");
            content.Add(new StringContent(model.Quantity.ToString()), "Quantity");
            content.Add(new StringContent(model.BrandId.ToString()), "BrandId");
            content.Add(new StringContent(model.Warranty.ToString()!), "Warranty");

            foreach (var category in model.CategoryIds!)
            {
                content.Add(new StringContent(category.ToString()), "CategoryIds");
            }
            // Thêm hình ảnh chính
            if (ImageUrl != null)
            {
                var mainImageContent = new StreamContent(ImageUrl.OpenReadStream())
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(ImageUrl.ContentType) }
                };
                content.Add(mainImageContent, "ImageUrl", ImageUrl.FileName);
            }

            // Thêm các hình ảnh phụ
            if (additionalImages != null)
            {
                foreach (var additionalImage in additionalImages)
                {
                    if (additionalImage.Length > 0) // Kiểm tra nếu hình ảnh không rỗng
                    {
                        var additionalImageContent = new StreamContent(additionalImage.OpenReadStream())
                        {
                            Headers = { ContentType = new MediaTypeHeaderValue(additionalImage.ContentType) }
                        };
                        content.Add(additionalImageContent, "additionalImages", additionalImage.FileName);
                    }
                }
            }

            var httpClient = _httpClientFactory.CreateClient(); // Tạo HttpClient từ IHttpClientFactory
            var apiUrl = $"{Url}/api/Products/CreateProduct";

            // Gửi request tới API
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Product"); // Chuyển hướng đến trang sản phẩm
            }

            ModelState.AddModelError("", "Failed to create product. Please try again."); // Thêm lỗi vào ModelState
            return RedirectToAction("Error", "Home"); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.ApiBaseUrl = _config["ApiSettings:BaseUrl"];
            var product = await _apiHelper.SendGetRequestAsync<ProductResponseDto>($"/api/Products/GetProductById?productId={id}");
            var categories = await _apiHelper.SendGetRequestAsync<IEnumerable<CategoryResponseDto>>("/api/Categories");
            var brands = await _apiHelper.SendGetRequestAsync<IEnumerable<BrandResponseDto>>("/api/Brands");

            if (product is null)
            {
                return RedirectToAction("Index", "Product");
            }

            var model = new CreateProductViewModel
            {
                Product = product,
                Categories = categories ?? [],
                Brands = brands ?? []
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProductRequestDto model, IFormFile mainImage, IList<IFormFile> additionalImages, List<string> oldImageUrls)
        {
            var Url = _config["ApiSettings:BaseUrl"];
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = $"{Url}/api/Products/UpdateProduct";
            var content = new MultipartFormDataContent();

            // Thêm thông tin sản phẩm vào content
            content.Add(new StringContent(model.ProductId.ToString()), "ProductId");
            content.Add(new StringContent(model.Name ?? ""), "Name");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.MetaDescription ?? ""), "MetaDescription");
            content.Add(new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price");
            content.Add(new StringContent(model.PromotionPrice?.ToString(CultureInfo.InvariantCulture) ?? "0"), "PromotionPrice");
            content.Add(new StringContent(model.Quantity.ToString()), "Quantity");
            content.Add(new StringContent(model.BrandId.ToString()), "BrandId");
            content.Add(new StringContent(model.CreatedDate.ToString("o")), "CreatedDate");
            content.Add(new StringContent(model.Warranty.ToString() ?? ""), "Warranty");

            foreach (var categoryId in model.CategoryIds ?? new List<Guid>())
            {
                content.Add(new StringContent(categoryId.ToString()), "CategoryIds");
            }

            // Xử lý ảnh chính
            if (mainImage != null)
            {
                oldImageUrls?.RemoveAt(0);  // Xóa ảnh cũ đầu tiên nếu có mainImage mới
                var mainImageContent = new StreamContent(mainImage.OpenReadStream())
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(mainImage.ContentType) }
                };
                content.Add(mainImageContent, "mainImage", mainImage.FileName);
            }

            // Xử lý ảnh phụ
            if (additionalImages != null)
            {
                foreach (var image in additionalImages)
                {
                    if (image.Length > 0)
                    {
                        var imageContent = new StreamContent(image.OpenReadStream())
                        {
                            Headers = { ContentType = new MediaTypeHeaderValue(image.ContentType) }
                        };
                        content.Add(imageContent, "additionalImages", image.FileName);
                    }
                }
            }

            // Chuyển oldImageUrls thành chuỗi JSON và thêm vào content
            if (oldImageUrls?.Count > 0)
            {
                var oldImageUrlsJson = JsonSerializer.Serialize(oldImageUrls);
                content.Add(new StringContent(oldImageUrlsJson), "oldImageUrlsJson");
            }

            // Gửi request
            var response = await httpClient.PutAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Product");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}, Message: {errorMessage}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
