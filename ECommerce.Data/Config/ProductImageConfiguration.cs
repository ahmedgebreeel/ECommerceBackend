using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageUrl)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500);

            builder.HasIndex(pi => pi.ProductId)
                .IsUnique()
                .HasFilter("[IsMain] = 1");

            builder.HasData(
                new ProductImage
                {
                    Id = 1,
                    ProductId = 1,
                    ImageUrl = "https://picsum.photos/seed/iphone14-main/200", // MAIN
                    IsMain = true
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = 1,
                    ImageUrl = "https://picsum.photos/seed/iphone14-2/200",
                    IsMain = false
                },
                new ProductImage
                {
                    Id = 3,
                    ProductId = 2,
                    ImageUrl = "https://picsum.photos/seed/galaxys23-main/200",
                    IsMain = true
                },
                new ProductImage
                {
                    Id = 4,
                    ProductId = 2,
                    ImageUrl = "https://picsum.photos/seed/galaxys23-2/200",
                    IsMain = false
                },
                new ProductImage
                {
                    Id = 5,
                    ProductId = 3,
                    ImageUrl = "https://picsum.photos/seed/sonyxm5-main/200",
                    IsMain = true
                },
                new ProductImage
                {
                    Id = 6,
                    ProductId = 3,
                    ImageUrl = "https://picsum.photos/seed/sonyxm5-2/200",
                    IsMain = false
                },
                new ProductImage
                {
                    Id = 7,
                    ProductId = 4,
                    ImageUrl = "https://picsum.photos/seed/nikeairmax-main/200",
                    IsMain = true
                },
                new ProductImage
                {
                    Id = 8,
                    ProductId = 4,
                    ImageUrl = "https://picsum.photos/seed/nikeairmax-2/200",
                    IsMain = false
                },
                new ProductImage
                {
                    Id = 9,
                    ProductId = 5,
                    ImageUrl = "https://picsum.photos/seed/adidastee-main/200",
                    IsMain = true
                },
                new ProductImage
                {
                    Id = 10,
                    ProductId = 5,
                    ImageUrl = "https://picsum.photos/seed/adidastee-2/200",
                    IsMain = false
                }
            );
        }
    }
}
