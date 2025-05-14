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
            CreateMap<Category, CategoryDetailResponseDto>()
                 .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src =>
                     src.ProductCategories!.Any() ?
                     src.ProductCategories!.Select(pc => pc.Product).Where(p => p != null).Min(p => p.Price) : 0))
                 .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src =>
                     src.ProductCategories!.Any() ?
                     src.ProductCategories!.Select(pc => pc.Product).Where(p => p != null).Max(p => p.Price) : 0))
                 .ForMember(dest => dest.ProductStock, opt => opt.MapFrom(src =>
                     src.ProductCategories!.Any() ?
                     src.ProductCategories!.Select(pc => pc.Product).Where(p => p != null).Sum(p => p.Quantity) : 0));
            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Brand, BrandResponseDto>();
            CreateMap<BrandRequestDto, Brand>();
            CreateMap<Brand, BrandDetailResponseDto>()
              .ForMember(dest => dest.StockAvailable, opt => opt.MapFrom(src => src.Products!.Sum(p => p.Quantity)))
              .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Products!.Sum(p => p.Quantity * p.Price)));
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
            CreateMap<Promotion, PromotionResponseDto>();
            CreateMap<PromotionRequestDto, Promotion>();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Transaction!.PaymentMethod!.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Promotion!.Code))
                .ForMember(dest => dest.totalProduct, opt => opt.MapFrom(src => src.OrderDetails!.Select(od => od.ProductId).Distinct().Count()));
            CreateMap<Order, OrderDetailResponseDto>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderDetails!.Select(od => new ProductDetailOrder
                {
                    PromotionPrice = od.Product.PromotionPrice,
                    ImageUrl = od.Product.ImageUrl,
                    Warranty = od.Product.Warranty,
                    Name = od.Product.Name,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity
                })))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.UserInfo, opt => opt.MapFrom(src => new UserInfoDto
                {
                    FirstName = src.User!.UserInfo!.FirstName!,
                    LastName = src.User.UserInfo.LastName!,
                    PhoneNumber = src.User.PhoneNumber!,
                    Address = src.User.UserInfo.Address!,
                    Gender = src.User.UserInfo.Gender,
                }));
        }
    }
}
