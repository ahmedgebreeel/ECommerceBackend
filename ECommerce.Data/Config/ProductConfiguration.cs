using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(200);

            builder.Property(p => p.Description).HasMaxLength(1000);

            builder.Property(p => p.Price).HasPrecision(18, 2);

            builder.Property(p => p.Version).IsRowVersion();

            builder.HasQueryFilter(p => !p.IsDeleted);

            //One to Many Relationship with ProductImage ( Product (1) -> (N) ProductImage )
            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //One to Many relation with CartItems ( Product (1) -> (N) CartItem )
            builder.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //One To Many Relation with WishlistItem ( Product (1) -> (N) WishListItem )
            builder.HasMany(p => p.WishlistItems)
                .WithOne(wi => wi.Product)
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
