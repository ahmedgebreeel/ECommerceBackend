using AutoMapper;
using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Brands;
using ECommerce.Business.DTOs.Categories;
using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.Orders;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.DTOs.Products;
using ECommerce.Core.Entities;

namespace ECommerce.Business.Mappings
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
