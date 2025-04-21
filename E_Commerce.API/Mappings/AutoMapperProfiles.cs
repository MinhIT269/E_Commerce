using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;

namespace E_Commerce.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand!.BrandName))
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src => src.ProductCategories!.Select(pc => pc.Category.CategoryName)))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages!.Select(pi => pi.ImageUrl)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Brand, BrandResponseDto>();
            CreateMap<BrandRequestDto, Brand>();
        }
    }
}
