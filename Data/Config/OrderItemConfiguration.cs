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

            builder.HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 999.99m
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 2,
                    ProductId = 3,
                    Quantity = 1,
                    UnitPrice = 349.99m
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = 3,
                    ProductId = 4,
                    Quantity = 1,
                    UnitPrice = 129.99m
                }
            );
        }
    }
}
