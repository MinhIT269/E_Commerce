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
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src => src.ProductCategories!.Select(pc => pc.Category!.CategoryName)))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages!.Select(pi => pi.ImageUrl)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Brand, BrandResponseDto>();
            CreateMap<BrandRequestDto, Brand>();
            CreateMap<Cart, CartResponseDto>()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));
            
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product!.ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product!.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<AddToCartRequestDto, Cart>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => new List<CartItem>{
                    new CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        ProductId = src.ProductId,
                        Quantity = src.Quantity
                    }
                }));

            CreateMap<AddToCartRequestDto, CartItem>()
                .ForMember(dest => dest.CartItemId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<UserInfo, UserInfoDto>().ReverseMap();
        }
    }
}
