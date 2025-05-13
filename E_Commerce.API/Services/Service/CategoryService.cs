using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return null;
            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto?> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.GetCategoryByNameAsync(name);
            if (category == null) return null;
            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> CreateCategoryAsync(CategoryRequestDto category)
        {
            var categoryDomain = _mapper.Map<Category>(category);
            return await _categoryRepository.CreateCategoryAsync(categoryDomain);
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CategoryRequestDto category)
        {
            var existing = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(category, existing);
            return await _categoryRepository.UpdateCategoryAsync(existing);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            return await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<(List<CategoryDetailResponseDto> categories, int totalRecords)> GetFilteredCategoriesAsync(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _categoryRepository.GetFilteredCategoriesQuery(searchQuery, sortCriteria, isDescending);

            var totalRecords = await query.CountAsync();

            var pagedCategories = await query.Skip((page - 1) * pageSize)
                                              .Take(pageSize)
                                              .ProjectTo<CategoryDetailResponseDto>(_mapper.ConfigurationProvider) 
                                              .ToListAsync();

            return (pagedCategories, totalRecords);
        }

        public async Task<int> GetTotalCategoriesAsync(string searchQuery)
        {
            var query = _categoryRepository.GetFilteredCategoriesQuery(searchQuery, "name", false);
            return await query.CountAsync();
        }

        public async Task<List<ProductResponseDto>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var products = await _categoryRepository.GetProductByCategoryIdAsync(categoryId);

            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task<bool> IsCategoryNameExistsAsync(string categoryName)
        {
            return await _categoryRepository.IsCategoryNameExistsAsync(categoryName);
        }
    }
}
