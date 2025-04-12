using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;

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
    }
}
