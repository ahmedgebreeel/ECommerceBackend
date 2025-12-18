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
                .HasColumnType("NVARCHAR(2000)");


            builder.HasIndex(pi => pi.ProductId)
                .IsUnique()
                .HasFilter("[IsMain] = 1");

            builder.HasQueryFilter(pi => !pi.Product.IsDeleted);

        }
    }
}
