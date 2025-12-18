using ECommerce.Core.Entities;
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

            builder.Property(o => o.Subtotal)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(o => o.ShippingFees)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(o => o.Taxes)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(o => o.TotalAmount)
                .HasColumnType("DECIMAL(18,2)");

            builder.Property(o => o.ShippingMethod)
                .HasConversion<string>()
                .HasMaxLength(50);

            //configure owned type (snapshot)
            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.WithOwner();
                sa.Property(sa => sa.Street).HasColumnName("ShippingStreet").HasColumnType("NVARCHAR(100)");
                sa.Property(sa => sa.City).HasColumnName("ShippingCity").HasColumnType("NVARCHAR(100)");
                sa.Property(sa => sa.State).HasColumnName("ShippingState").HasColumnType("NVARCHAR(100)");
                sa.Property(sa => sa.PostalCode).HasColumnName("ShippingPostalCode").HasColumnType("NVARCHAR(100)");
                sa.Property(sa => sa.Country).HasColumnName("ShippingCountry").HasColumnType("NVARCHAR(100)");
            });

            //one to many relation with OrderItems
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            //one to many relation with OrderTrackingMilestones
            builder.HasMany(o => o.OrderTrackingMilestones)
                .WithOne(otm => otm.Order)
                .HasForeignKey(otm => otm.OrderId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
