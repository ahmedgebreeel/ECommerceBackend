using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.Property(u => u.AvatarUrl).HasMaxLength(2000);
            builder.Property(u => u.FirstName).HasMaxLength(50);
            builder.Property(u => u.LastName).HasMaxLength(50);
            builder.Property(u => u.UserName).HasMaxLength(30).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();

            //One to One Relation with Wishlist ( ApplicationUser (1) -> (1) Wishlist )
            builder.HasOne(u => u.Wishlist)
                .WithOne(w => w.User)
                .HasForeignKey<Wishlist>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to One Relation with ShoppingCart ( ApplicationUser (1) -> (1) ShoppingCart )
            builder.HasOne(u => u.ShoppingCart)
                .WithOne(sc => sc.User)
                .HasForeignKey<ShoppingCart>(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to Many Relation with Order ( ApplicationUser (1) -> (N) Order )
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //One to Many Relation with Address ( ApplicationUser (1) -> (N) Address )
            builder.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to Many Relation with RefreshToken ( ApplicationUser (1) -> (N) RefreshToken)
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to Many Relation with Review ( ApplicationUser (1) -> (N) Review )
            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
