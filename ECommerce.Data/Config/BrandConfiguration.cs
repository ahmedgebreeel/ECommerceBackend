using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {

            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(50);


            builder.Property(x => x.Description)
                .HasColumnType("VARCHAR(MAX)");


            //one to many relation with Product
            builder.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


            //Seed Data
            builder.HasData(
                new Brand
                {
                    Id = 1,
                    Name = "Apple",
                    Description = "Consumer electronics and software"
                },
                new Brand
                {
                    Id = 2,
                    Name = "Samsung",
                    Description = "Electronics, appliances, and mobile devices"
                },
                new Brand
                {
                    Id = 3,
                    Name = "Sony",
                    Description = "Entertainment and consumer electronics"
                },
                new Brand
                {
                    Id = 4,
                    Name = "Nike",
                    Description = "Sportswear and footwear"
                },
                new Brand
                {
                    Id = 5,
                    Name = "Adidas",
                    Description = "Sportswear, apparel and accessories"
                }
            );
        }
    }
}
