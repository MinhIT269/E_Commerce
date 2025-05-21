using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using System.Text.RegularExpressions;

namespace E_Commerce.API.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper, IImageService imageService)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _imageService = imageService;
        }

        public async Task<List<ProductResponseDto>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
        {
            if (id == Guid.Empty) return null;
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<List<ProductResponseDto>?> GetProductsFromQuery(string? filterQuery, string sortBy, bool isAscending, int pageNumber, int pageSize, Guid? categoryId, Guid? brandId)
        {
            var query = (filterQuery ?? string.Empty).Trim();
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var products = await _productRepository.GetAllAsync(query, sortBy, isAscending, pageNumber, pageSize, categoryId, brandId);
            if (products == null) return null;
            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

            var productIds = productDtos.Select(p => p.ProductId).ToList();
            var soldQuantities = await _productRepository.GetSoldQuantitiesAsync(productIds);

            foreach (var dto in productDtos)
            {
                dto.SoldQuantity = soldQuantities.ContainsKey(dto.ProductId) ? soldQuantities[dto.ProductId] : 0;
            }

            return productDtos;
        }

        public async Task<List<ProductResponseDto>?> GetProductsFromBrand(Guid brandId, string filterQuery)
        {
            var query = filterQuery.Trim();
            if (brandId == Guid.Empty) return null;

            var products = await _productRepository.FindProductsByBrandAsync(query, brandId);

            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task<int> CountProductAsync(string? searchQuery, Guid? categoryId = null, Guid? brandId = null)
        {
            return await _productRepository.CountProductAsync(searchQuery?.Trim(), categoryId, brandId);
        }

        public async Task<List<ProductResponseDto>?> GetProductsByCategoryAsync(Guid categoryId, int count)
        {
            if (categoryId == Guid.Empty) return null;
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId, count);
            if (products == null) return null;
            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task DeleteAsync(Guid id)
        {
            var promotion = await _productRepository.GetProductByIdAsync(id);

            if (promotion != null)
            {
                await _productRepository.DeletePromotionAsync(promotion);
            }
            else
            {
                throw new Exception("Promotion not found");
            }
        }

        public async Task<int> GetTotalProduct()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Count;
        }
        public async Task<int> AvailableProducts()
        {
            return await _productRepository.GetAvailableProduct();
        }
        public async Task<int> GetLowStockProducts()
        {
            return await _productRepository.GetLowStockProducts();
        }
        public async Task<int> GetNewProducts()
        {
            return await _productRepository.GetNewProducts();
        }

        public async Task<bool> AddProductAsync(ProductRequestDto model, IFormFile mainImage, IList<IFormFile> additionalImages)
        {
            var imagePaths = new List<string>();

            if (mainImage != null && mainImage!.Length > 0)
            {
                imagePaths.Add(await _imageService.UploadImageAsync(mainImage, model.ProductId));
            }

            if (additionalImages != null)
            {
                foreach (var image in additionalImages.Where(img => img.Length > 0))
                {
                    imagePaths.Add(await _imageService.UploadImageAsync(image, model.ProductId));
                }
            }

            model.AdditionalImageUrls = imagePaths.Skip(1).ToList();
            var product = _mapper.Map<Product>(model);
            product.ImageUrl = imagePaths.FirstOrDefault();

            return await _productRepository.AddProductAsync(product);
        }
        public async Task<bool> UpdateProductAsync(ProductRequestDto model, IFormFile? mainImage, IList<IFormFile>? additionalImages, List<string>? oldImageUrls)
        {
            var imagePaths = new List<string>();

            var product = await _productRepository.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại");
            }

            _mapper.Map(model, product);

            var allImageUrls = await _imageService.GetAllImageUrlsForProductAsync(model.ProductId);

            var oldImageSet = new HashSet<string>(oldImageUrls ?? new List<string>());  // Tạo một tập hợp các URL cũ để giữ lại

            var descriptionImageUrls = ExtractImageUrlsFromDescription(model.Description);
            foreach (var url in descriptionImageUrls)
            {
                oldImageSet.Add(url); // Nếu chưa có thì thêm vào oldImageSet
            }

            foreach (var imageUrl in allImageUrls)
            {
                if (!oldImageSet.Contains(imageUrl))
                {
                    await _imageService.DeleteImageAsync(imageUrl); // Xóa ảnh nếu không nằm trong oldImageUrls
                }
                else if (!descriptionImageUrls.Contains(imageUrl))
                {
                    imagePaths.Add(imageUrl);
                }
            }
            if (mainImage != null && mainImage.Length > 0)    // Xử lý ảnh chính nếu có ảnh mới
            {
                var mainImageUrl = await _imageService.UploadImageAsync(mainImage, model.ProductId);
                product.ImageUrl = mainImageUrl;  // Cập nhật URL của ảnh chính
            }
            else
            {
                var firstOldImageUrl = oldImageUrls[0];

                // Xóa phần tử đầu tiên này nếu nó tồn tại trong imagePaths
                imagePaths.Remove(firstOldImageUrl);
            }

            if (additionalImages != null && additionalImages.Count > 0)
            {
                foreach (var image in additionalImages.Where(img => img.Length > 0))
                {
                    imagePaths.Add(await _imageService.UploadImageAsync(image, model.ProductId));
                }
            }

            if (product.ProductImages == null)
            {
                product.ProductImages = new List<ProductImage>();
            }
            foreach (var url in imagePaths)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ProductImageId = Guid.NewGuid(),
                    ProductId = model.ProductId,
                    ImageUrl = url
                });
            }
            return await _productRepository.UpdateProduct(product);
        }
        private List<string> ExtractImageUrlsFromDescription(string description)
        {
            var urls = new List<string>();
            var regex = new Regex("<img[^>]+?src=[\"'](?<url>.+?)[\"'][^>]*>", RegexOptions.IgnoreCase);

            var matches = regex.Matches(description);
            foreach (Match match in matches)
            {
                var url = match.Groups["url"].Value;
                urls.Add(url);
            }

            return urls;
        }
    }
}
