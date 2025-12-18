using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(p => p.Description)
                .HasColumnType("NVARCHAR(MAX)");

            builder.Property(p => p.Price)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(p => p.Version)
                .IsRowVersion();

            builder.HasQueryFilter(p => !p.IsDeleted);

            //one to many Relationship with ProductImage
            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //one to many Relationship with OrderItem
            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            //one to many relation with CartItems
            builder.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
