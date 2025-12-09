using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.API.Entities;

namespace MyApp.API.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasColumnType("VARCHAR(MAX)");

            builder.Property(p => p.Price)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(p => p.ImageUrl)
                .HasColumnType("VARCHAR")
                .HasMaxLength(500);


            //one to many Relationship with ProductImage
            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //one to many Relationship with OrderItem
            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 14",
                    Description = "Latest Apple smartphone",
                    Price = 999.99m,
                    StockQuantity = 50,
                    ImageUrl = "https://picsum.photos/seed/iphone14-main/200", // MAIN IMAGE
                    CategoryId = 1,
                    BrandId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S23",
                    Description = "Flagship Samsung smartphone",
                    Price = 899.99m,
                    StockQuantity = 40,
                    ImageUrl = "https://picsum.photos/seed/galaxys23-main/200",
                    CategoryId = 1,
                    BrandId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Sony WH-1000XM5",
                    Description = "Noise-cancelling wireless headphones",
                    Price = 349.99m,
                    StockQuantity = 30,
                    ImageUrl = "https://picsum.photos/seed/sonyxm5-main/200",
                    CategoryId = 1,
                    BrandId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Nike Air Max",
                    Description = "Popular athletic sneakers",
                    Price = 129.99m,
                    StockQuantity = 100,
                    ImageUrl = "https://picsum.photos/seed/nikeairmax-main/200",
                    CategoryId = 2,
                    BrandId = 4
                },
                new Product
                {
                    Id = 5,
                    Name = "Adidas Running Tee",
                    Description = "Lightweight running T-shirt",
                    Price = 29.99m,
                    StockQuantity = 200,
                    ImageUrl = "https://picsum.photos/seed/adidastee-main/200",
                    CategoryId = 2,
                    BrandId = 5
                }
            );

        }
    }
}
