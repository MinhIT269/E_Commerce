using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;

namespace E_Commerce.API.Services.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<List<BrandResponseDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllBrandsAsync();
            return _mapper.Map<List<BrandResponseDto>>(brands);
        }

        public async Task<BrandResponseDto?> GetBrandByIdAsync(Guid id)
        {
            var brands = await _brandRepository.GetBrandByIdAsync(id);
            if (brands == null) return null;
            return _mapper.Map<BrandResponseDto>(brands);
        }

        public async Task<BrandResponseDto?> GetBrandByNameAsync(string name)
        {
            var brands = await _brandRepository.GetBrandByNameAsync(name.Trim());
            if (brands == null) return null;
            return _mapper.Map<BrandResponseDto>(brands);
        }

        public async Task<bool> CreateBrandAsync(BrandRequestDto brandDto)
        {
            var brand = _mapper.Map<Brand>(brandDto);
            return await _brandRepository.CreateBrandAsync(brand);
        }

        public async Task<bool> UpdateBrandAsync(Guid id, BrandRequestDto brand)
        {
            var existingBrand = await _brandRepository.GetBrandByIdAsync(id);
            if (existingBrand == null) return false;

            _mapper.Map(brand, existingBrand);
            return await _brandRepository.UpdateBrandAsync(existingBrand);
        }
        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            return await _brandRepository.DeleteBrandAsync(id);
        }
    }
}
