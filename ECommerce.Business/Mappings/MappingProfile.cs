using AutoMapper;
using ECommerce.Business.DTOs.Addresses.Requests;
using ECommerce.Business.DTOs.Addresses.Responses;
using ECommerce.Business.DTOs.Auth.Requests;
using ECommerce.Business.DTOs.Brands.Requests;
using ECommerce.Business.DTOs.Brands.Responses;
using ECommerce.Business.DTOs.Categories.Requests;
using ECommerce.Business.DTOs.Categories.Responses;
using ECommerce.Business.DTOs.Checkout.Responses;
using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.OrderTrackingMilestones;
using ECommerce.Business.DTOs.ProductAttribute;
using ECommerce.Business.DTOs.ProductImages.Responses;
using ECommerce.Business.DTOs.Products.Requests;
using ECommerce.Business.DTOs.Products.Responses;
using ECommerce.Business.DTOs.Reviews.Responses;
using ECommerce.Business.DTOs.ShoppingCart.Responses;
using ECommerce.Business.DTOs.WishlistItem;
using ECommerce.Core.Entities;

namespace ECommerce.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Auth Mapping
            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(d => d.Created, o => o.MapFrom(s => DateTime.UtcNow));

            //Brand Mapping
            CreateMap<Brand, AdminBrandSummaryDto>()
                .ForMember(d => d.ProductsCount, o => o.MapFrom(s => s.Products.Count));

            CreateMap<Brand, BrandDetailsResponse>();

            CreateMap<CreateBrandRequest, Brand>();

            CreateMap<UpdateBrandRequest, Brand>();

            CreateMap<Brand, BrandSummaryDto>()
                .ForMember(d => d.ProductsCount, o => o.MapFrom(s => s.Products.Count));

            //Category Mapping
            CreateMap<Category, AdminCategorySummaryDto>()
                .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.Parent == null ? "Root Category" : s.Parent.Name))
                .ForMember(d => d.PathFromRoot, o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));

            CreateMap<Category, CategoryDetailsResponse>()
                .ForMember(d => d.SubcategoriesNames, o => o.MapFrom(s => s.SubCategories.Select(c => c.Name)))
                .ForMember(d => d.PathFromRoot, o => o.MapFrom(s => $"Root\\{s.HierarchyPath}"));

            CreateMap<CreateCategoryRequest, Category>()
                .ForMember(d => d.Created, o => o.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateCategoryRequest, Category>()
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow));

            CreateMap<Category, CategorySummaryDto>()
                .ForMember(d => d.Subcategories, o => o.Ignore());

            //Product Mapping
            CreateMap<Product, AdminProductSummaryDto>()
                .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => s.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.InStock, o => o.MapFrom(s => s.StockQuantity > 0));

            CreateMap<Product, AdminProductDetailsResponse>();

            CreateMap<CreateProductRequest, Product>()
                .ForMember(d => d.Created, o => o.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow)); ;

            CreateMap<Product, ProductSummaryDto>()
                .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => s.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.BrandedName, o => o.MapFrom(s => s.Brand.Name + " " + s.Name));

            CreateMap<Product, ProductDetailsResponse>()
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.CareInstructions, o => o.MapFrom(s => s.CareInstructions.Select(ci => ci.Instruction).ToList()))
                .ForMember(d => d.Features, o => o.MapFrom(s => s.Features.Select(f => f.Feature).ToList()));


            //ProductAttribut Mapping
            CreateMap<ProductAttribute, ProductAttributeDto>();


            //ProductImage Mapping
            CreateMap<ProductImage, ProductImageDto>();

            //ShoppingCart Mapping
            CreateMap<ShoppingCart, CartResponse>()
                .ForMember(d => d.CartTotal, o => o.MapFrom(s => s.Items.Sum(i => i.Quantity * i.Product.Price)));

            CreateMap<CartItem, OrderItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.Product.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.ProductPrice, o => o.MapFrom(s => s.Product.Price))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.Quantity * s.Product.Price));

            //Wishlist Mapping
            CreateMap<WishlistItem, WishlistItemSummaryDto>()
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.Product.Images.Where(pi => pi.IsMain).Select(pi => pi.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductPrice, o => o.MapFrom(s => s.Product.Price))
                .ForMember(d => d.InStock, o => o.MapFrom(s => s.Product.StockQuantity > 0))
                .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.Product.IsDeleted));

            //Order Mapping
            CreateMap<Order, AdminOrderSummaryDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.Items.Count));

            CreateMap<Order, OrderDetailsResponse>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FirstName + " " + s.User.LastName));

            CreateMap<Order, OrderSummaryDto>()
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.Items.Count));

            //OrderItem Mapping
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.OrderedProduct.Id))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.OrderedProduct.Name))
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.OrderedProduct.ThumbnailUrl))
                .ForMember(d => d.ProductPrice, o => o.MapFrom(s => s.OrderedProduct.Price));

            //OrderTrackingMilestone Mapping
            CreateMap<OrderTrackingMilestone, OrderTrackingMilestoneDto>();

            //Address Mapping
            CreateMap<Address, AddressSummaryDto>();

            CreateMap<CreateAddressRequest, Address>()
                .ForMember(d => d.Created, o => o.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateAddressRequest, Address>()
                .ForMember(d => d.Updated, o => o.MapFrom(s => DateTime.UtcNow));

            CreateMap<Address, OrderAddress>();

            //Checkout Mapping
            CreateMap<ShoppingCart, CheckoutPreviewResponse>();

            CreateMap<CartItem, OrderItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Total, o => o.MapFrom(s => s.Quantity * s.Product.Price))
                .ForMember(d => d.OrderedProduct, o => o.MapFrom(s => new OrderedProduct
                {
                    Id = s.Product.Id,
                    Name = s.Product.Name,
                    Description = s.Product.OverviewDescription,
                    Price = s.Product.Price,
                    ThumbnailUrl = s.Product.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).First()
                }));

            CreateMap<Order, OrderConfirmationResponse>()
                .ForMember(d => d.EstimatedDeliveryDateStart, o => o.MapFrom(s => s.Created.AddDays(s.ShippingMethod == Core.Enums.ShippingMethod.Express ? 2 : 5)))
                .ForMember(d => d.EstimatedDeliveryDateEnd, o => o.MapFrom(s => s.Created.AddDays(s.ShippingMethod == Core.Enums.ShippingMethod.Express ? 3 : 7)))
                .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.ItemsCount, o => o.MapFrom(s => s.Items.Count));

            //Reviews Mapping
            CreateMap<Review, ReviewSummaryDto>()
                .ForMember(d => d.ProductThumbnailUrl, o => o.MapFrom(s => s.Product.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).FirstOrDefault()))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

            CreateMap<Review, ReviewProductSummaryDto>()
                .ForMember(d => d.UserAvatarUrl, o => o.MapFrom(s => s.User.AvatarUrl))
                .ForMember(d => d.UserName, o => o.MapFrom(s => $"{s.User.FirstName} {s.User.LastName}"));

            //Users Mapping
            CreateMap<ApplicationUser, UserDetailsResponse>();

        }
    }
}
