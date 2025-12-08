using AutoMapper;
using MyApp.API.DTOs.Auth;
using MyApp.API.DTOs.Brands;
using MyApp.API.DTOs.Categories;
using MyApp.API.DTOs.OrderItems;
using MyApp.API.DTOs.Orders;
using MyApp.API.DTOs.ProductImages;
using MyApp.API.DTOs.Products;
using MyApp.API.Entities;

namespace MyApp.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Brand Mapping
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandDto, Brand>();
            CreateMap<UpdateBrandDto, Brand>();

            //Category Mapping
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            //Product Mapping
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<AddProductImageDto, ProductImage>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.IsMain, opt => opt.MapFrom(src => false));

            //Order Mapping
            CreateMap<Order, OrderDto>();
            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>();

            //User Mapping
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>();


        }
    }
}
