using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.API.Entities;

namespace MyApp.API.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("DECIMAL(18,2)");

            // 🔥 SEED ORDER ITEMS
            builder.HasData(

                // Order 1 (iPhone 14)
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,             // iPhone 14
                    Quantity = 1,
                    UnitPrice = 999.99m
                },

                // Order 2 (Sony WH-1000XM5)
                new OrderItem
                {
                    Id = 2,
                    OrderId = 2,
                    ProductId = 3,             // Sony WH-1000XM5
                    Quantity = 1,
                    UnitPrice = 349.99m
                },

                // Order 3 (Nike Air Max)
                new OrderItem
                {
                    Id = 3,
                    OrderId = 3,
                    ProductId = 4,             // Nike Air Max
                    Quantity = 1,
                    UnitPrice = 129.99m
                }
            );
        }
    }
}
