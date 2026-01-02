using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderTrackingMilestoneConfiguration : IEntityTypeConfiguration<OrderTrackingMilestone>
    {
        public void Configure(EntityTypeBuilder<OrderTrackingMilestone> builder)
        {
            builder.HasQueryFilter(otm => !otm.Order.User.IsDeleted);

            builder.Property(otm => otm.Status)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
