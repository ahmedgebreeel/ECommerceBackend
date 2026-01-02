using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasQueryFilter(a => !a.User.IsDeleted);

            builder.Property(a => a.FullName).HasMaxLength(50);
            builder.Property(a => a.MobileNumber).HasColumnType("VARCHAR").HasMaxLength(15);
            builder.Property(a => a.Label).HasMaxLength(50);
            builder.Property(a => a.Street).HasMaxLength(60);
            builder.Property(a => a.Building).HasMaxLength(50);
            builder.Property(a => a.City).HasMaxLength(50);
            builder.Property(a => a.District).HasMaxLength(50);
            builder.Property(a => a.Governorate).HasMaxLength(50);
            builder.Property(a => a.Country).HasMaxLength(100);
            builder.Property(a => a.ZipCode).HasMaxLength(50);
            builder.Property(a => a.Hints).HasMaxLength(100);

            //User Can have only one Address as default
            builder.HasIndex(pi => pi.UserId)
                .IsUnique()
                .HasFilter("[IsDefault] = 1");
        }
    }
}
