using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("DECIMAL(18,2)");
            builder.OwnsOne(oi => oi.ProductOrdered, po =>
            {
                po.WithOwner();
                po.Property(po => po.ProductId).HasColumnName("OrderedProductId");
                po.Property(po => po.ProductName).HasColumnName("OrderedProductName").HasMaxLength(100);
                po.Property(po => po.PictureUrl).HasColumnName("OrderedProductThumbnailUrl");
            });
        }
    }
}
