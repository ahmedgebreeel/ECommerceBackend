using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.API.Entities;

namespace MyApp.API.Data.Config
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(i => i.ImageUrl)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500);

            builder.HasData(
                // Product 1
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

                // Product 2
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

                // Product 3
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

                // Product 4
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

                // Product 5
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
