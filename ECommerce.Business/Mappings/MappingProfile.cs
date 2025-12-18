using AutoMapper;
using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Brands;
using ECommerce.Business.DTOs.Categories;
using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.OrderTrackingMilestones;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.DTOs.Products.Store;
using ECommerce.Business.DTOs.ShoppingCart;
using ECommerce.Business.DTOs.Users.Auth;
using ECommerce.Core.Entities;

namespace ECommerce.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Brand Mapping
            CreateMap<Brand, BrandDto>()
                .ForMember(dest => dest.ProductsCount, opt => opt.MapFrom(src => src.Products.Count));
            CreateMap<CreateBrandDto, Brand>();
            CreateMap<UpdateBrandDto, Brand>();

            //Category Mapping
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            //Product Mapping
            CreateMap<Product, AdminProductDto>()
                .ForMember(dest => dest.ThumbnailUrl,
                opt => opt.MapFrom(src => src.Images
                                                                .Where(pi => pi.IsMain)
                                                                .Select(pi => pi.ImageUrl)
                                                                .FirstOrDefault()))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName,
                opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.InStock,
                opt => opt.MapFrom(src => src.StockQuantity > 0));
            CreateMap<Product, AdminProductDetailsDto>();
            CreateMap<AdminCreateProductDto, Product>();
            CreateMap<AdminUpdateProductDto, Product>();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ThumbnailUrl,
                opt => opt.MapFrom(src => src.Images
                                                                .Where(pi => pi.IsMain)
                                                                .Select(pi => pi.ImageUrl)
                                                                .FirstOrDefault()))
                .ForMember(dest => dest.CategoryBreadcrumb,
                opt => opt.MapFrom(src => src.Category.HierarchyPath))
                .ForMember(dest => dest.BrandedName,
                opt => opt.MapFrom(src => src.Brand.Name + " " + src.Name));
            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.BrandName,
                opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));

            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();

            //Order Mapping
            CreateMap<Order, AdminOrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));
            CreateMap<Order, AdminOrderDetailsDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
            CreateMap<Order, OrderDto>();

            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom(src => src.ProductOrdered.ProductId))
                .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.ProductOrdered.ProductName))
                .ForMember(dest => dest.ProductThumbnailUrl, opt
                => opt.MapFrom(src => src.ProductOrdered.PictureUrl));

            //OrderTrackingMilestone Mapping
            CreateMap<OrderTrackingMilestone, OrderTrackingMilestoneDto>();

            //User Mapping
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserSessionDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            //CreateMap<ApplicationUser, UserDetailsDto>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + src.LastName))
            //    .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count));
            //CreateMap<ApplicationUser, UserManagementDto>()
            //    .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count));
            //CreateMap<UpdateUserDto, ApplicationUser>();


            //CreateMap<ApplicationUser, UserManagementDto>();
            //CreateMap<UpdateUserProfileDto, ApplicationUser>();

            //Address Mapping
            CreateMap<Address, AddressDto>();
            CreateMap<Address, AddressWithUserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<CreateAddressDto, Address>();
            CreateMap<UpdateAddressDto, Address>();
            CreateMap<Address, OrderAddress>();

            //ShoppingCart Mapping
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ThumbnailUrl,
                opt => opt.MapFrom(src => src.Product.Images
                                                                .Where(pi => pi.IsMain)
                                                                .Select(pi => pi.ImageUrl)
                                                                .FirstOrDefault()))
                .ForMember(dest => dest.BrandedName,
                opt => opt.MapFrom(src => src.Product.Brand.Name + " " + src.Product.Name))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Product.Price));



            //Checkout Mapping
            CreateMap<CartItem, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductOrdered, opt => opt.MapFrom(src => new ProductItemOrdered
                {
                    ProductId = src.ProductId,
                    ProductName = src.Product.Name,
                    PictureUrl = src.Product.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).FirstOrDefault()
                }))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price));
        }
    }
}
