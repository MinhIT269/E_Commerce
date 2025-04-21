using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;

namespace E_Commerce.API.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
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
            return _mapper.Map<List<ProductResponseDto>>(products);
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
    }
}
