using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ECommerce.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        //Products Inventory
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }

        //Wishlist
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }

        //Shopping Cart
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        //Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderTrackingMilestone> OrderTrackingMilestones { get; set; }

        //User
        public DbSet<Address> Addresses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
