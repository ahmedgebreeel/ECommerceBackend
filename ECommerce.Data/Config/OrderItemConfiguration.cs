using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasQueryFilter(oi => !oi.Order.User.IsDeleted);

            builder.Property(oi => oi.Total).HasPrecision(18, 2);

            //Owned Entity (Snapshot)
            builder.OwnsOne(oi => oi.OrderedProduct, p =>
            {
                p.WithOwner();
                p.Property(p => p.Name).HasMaxLength(200);
                p.Property(p => p.Description).HasMaxLength(1000);
                p.Property(p => p.Price).HasPrecision(18, 2);

            });
        }
    }
}
