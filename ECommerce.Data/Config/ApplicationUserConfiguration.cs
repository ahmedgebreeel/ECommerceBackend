using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(50);
            builder.Property(u => u.LastName).HasMaxLength(50);
            builder.Property(u => u.UserName).HasMaxLength(30).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();


            //one to many relation with orders
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //one to many relation with addresses
            builder.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //one to one relation with ShoppingCart
            builder.HasOne(u => u.ShoppingCart)
                .WithOne(sc => sc.User)
                .HasForeignKey<ShoppingCart>(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //one to many relation with RefreshTokens
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
