using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.TotalAmount)
                .HasColumnType("DECIMAL(18,2)");

            //one to many relation with OrderItems
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasData(
                new Order
                {
                    Id = 1,
                    Created = new DateTime(2024, 12, 1, 10, 00, 00),
                    Status = OrderStatus.Pending,
                    TotalAmount = 999.99m
                },
                new Order
                {
                    Id = 2,
                    Created = new DateTime(2024, 12, 2, 14, 30, 00),
                    Status = OrderStatus.Processing,
                    TotalAmount = 349.99m

                },
                new Order
                {
                    Id = 3,
                    Created = new DateTime(2024, 12, 3, 18, 45, 00),
                    Status = OrderStatus.Delivered,
                    TotalAmount = 129.99m
                }
            );
        }
    }
}
