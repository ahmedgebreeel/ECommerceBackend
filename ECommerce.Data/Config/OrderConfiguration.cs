using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasQueryFilter(o => !o.User.IsDeleted);

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

            builder.Property(o => o.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(50);

            //Owned Entity (Snapshot)
            builder.OwnsOne(o => o.ShippingAddress, a =>
            {
                a.WithOwner();
                a.Property(a => a.FullName).HasMaxLength(50);
                a.Property(a => a.MobileNumber).HasColumnType("VARCHAR").HasMaxLength(15);
                a.Property(a => a.Label).HasMaxLength(50);
                a.Property(a => a.Street).HasMaxLength(60);
                a.Property(a => a.Building).HasMaxLength(50);
                a.Property(a => a.City).HasMaxLength(50);
                a.Property(a => a.District).HasMaxLength(50);
                a.Property(a => a.Governorate).HasMaxLength(50);
                a.Property(a => a.Country).HasMaxLength(100);
                a.Property(a => a.ZipCode).HasMaxLength(50);
                a.Property(a => a.Hints).HasMaxLength(100);

            });

            //One to Many Relation with OrderItem ( Order (1) -> (N) OrderItem )
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to Many Relation With OrderTrackingMilestone ( Order (1) -> (N) OrderTrackingMilestone )
            builder.HasMany(o => o.OrderTrackingMilestones)
                .WithOne(otm => otm.Order)
                .HasForeignKey(otm => otm.OrderId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
