using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                // 1. Ensure Database Created
                await context.Database.MigrateAsync();
                // 2. Seed Identity (Roles & Admin)
                await SeedRolesAndAdminAsync(userManager, roleManager);
                // 3. Seed Catalog (Brands & Categories)
                await SeedBrandsAsync(context);
                await SeedCategoriesAsync(context);
                // 4. Seed Products (With IsFeatured)
                await SeedProductsAsync(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedRolesAndAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer", "Seller" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (await userManager.FindByEmailAsync("admin@myapp.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    FirstName = "Abdelrhman",
                    LastName = "Mady",
                    UserName = "admin",
                    Email = "admin@myapp.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        private static async Task SeedBrandsAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            var brands = new List<Brand>
            {
                new() { Name = "Apple", Description = "Consumer electronics and software" },
                new() { Name = "Samsung", Description = "Electronics, appliances, and mobile devices" },
                new() { Name = "Sony", Description = "Entertainment and consumer electronics" },
                new() { Name = "Nike", Description = "Sportswear and footwear" },
                new() { Name = "Adidas", Description = "Sportswear, apparel and accessories" }
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new() { Name = "Electronics", Description = "Devices and gadgets" },
                new() { Name = "Clothing", Description = "Men and women clothing" },
                new() { Name = "Books", Description = "Fiction, non-fiction, academic" },
                new() { Name = "Home", Description = "Furniture & home accessories" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            // Fetch IDs dynamically to ensure foreign key integrity
            var appleId = (await context.Brands.FirstAsync(b => b.Name == "Apple")).Id;
            var samsungId = (await context.Brands.FirstAsync(b => b.Name == "Samsung")).Id;
            var sonyId = (await context.Brands.FirstAsync(b => b.Name == "Sony")).Id;
            var nikeId = (await context.Brands.FirstAsync(b => b.Name == "Nike")).Id;
            var adidasId = (await context.Brands.FirstAsync(b => b.Name == "Adidas")).Id;

            var elecId = (await context.Categories.FirstAsync(c => c.Name == "Electronics")).Id;
            var clothId = (await context.Categories.FirstAsync(c => c.Name == "Clothing")).Id;

            var products = new List<Product>
            {
                // --- ELECTRONICS ---
                new() {
                    Name = "MacBook Pro M3",
                    Description = "The most advanced Mac laptop.",
                    Price = 1999.99m,
                    StockQuantity = 15,
                    CategoryId = elecId,
                    BrandId = appleId,
                    IsFeatured = true, // <--- FEATURED
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/macbook/300/300" } ]
                },
                new() {
                    Name = "iPad Air 5",
                    Description = "Light. Bright. Full of might.",
                    Price = 599.99m,
                    StockQuantity = 40,
                    CategoryId = elecId,
                    BrandId = appleId,
                    IsFeatured = false,
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/ipadair/300/300" } ]
                },
                new() {
                    Name = "Samsung Odyssey Monitor",
                    Description = "49-inch curved gaming monitor.",
                    Price = 1299.99m,
                    StockQuantity = 8,
                    CategoryId = elecId,
                    BrandId = samsungId,
                    IsFeatured = true, // <--- FEATURED
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/monitor/300/300" } ]
                },
                new() {
                    Name = "Sony Alpha a7 IV",
                    Description = "Hybrid full-frame mirrorless camera.",
                    Price = 2499.00m,
                    StockQuantity = 5,
                    CategoryId = elecId,
                    BrandId = sonyId,
                    IsFeatured = false,
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/sonycamera/300/300" } ]
                },
                new() {
                    Name = "PlayStation 5 Pro",
                    Description = "The next level of gaming console performance.",
                    Price = 499.99m,
                    StockQuantity = 0,
                    CategoryId = elecId,
                    BrandId = sonyId,
                    IsFeatured = true, // <--- FEATURED
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/ps5/300/300" } ]
                },

                // --- CLOTHING ---
                new() {
                    Name = "Nike Tech Fleece",
                    Description = "Premium lightweight warmth.",
                    Price = 110.00m,
                    StockQuantity = 50,
                    CategoryId = clothId,
                    BrandId = nikeId,
                    IsFeatured = true, // <--- FEATURED
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/nikefleece/300/300" } ]
                },
                new() {
                    Name = "Adidas Ultraboost Light",
                    Description = "Experience epic energy.",
                    Price = 190.00m,
                    StockQuantity = 35,
                    CategoryId = clothId,
                    BrandId = adidasId,
                    IsFeatured = false,
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/ultraboost/300/300" } ]
                },
                new() {
                    Name = "Nike Air Force 1",
                    Description = "The radiance lives on.",
                    Price = 115.00m,
                    StockQuantity = 100,
                    CategoryId = clothId,
                    BrandId = nikeId,
                    IsFeatured = true, // <--- FEATURED
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/af1/300/300" } ]
                },
                new() {
                    Name = "Adidas Trefoil Hoodie",
                    Description = "A cozy hoodie with the iconic logo.",
                    Price = 65.00m,
                    StockQuantity = 60,
                    CategoryId = clothId,
                    BrandId = adidasId,
                    IsFeatured = false,
                    Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/adishoodie/300/300" } ]
                },
                
                // --- FILLER ITEMS ---
                new() { Name = "Apple Pencil (2nd Gen)", Price = 129m, StockQuantity = 200, CategoryId = elecId, BrandId = appleId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/pencil/300/300" } ] },
                new() { Name = "Samsung Galaxy Buds2", Price = 149m, StockQuantity = 80, CategoryId = elecId, BrandId = samsungId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/buds/300/300" } ] },
                new() { Name = "Sony XM4 Headphones", Price = 278m, StockQuantity = 25, CategoryId = elecId, BrandId = sonyId, IsFeatured = true, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/xm4/300/300" } ] },
                new() { Name = "Nike Dri-FIT Shorts", Price = 35m, StockQuantity = 120, CategoryId = clothId, BrandId = nikeId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/shorts/300/300" } ] },
                new() { Name = "Adidas Gazelle", Price = 100m, StockQuantity = 45, CategoryId = clothId, BrandId = adidasId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/gazelle/300/300" } ] },
                new() { Name = "MacBook Air M2", Price = 1099m, StockQuantity = 10, CategoryId = elecId, BrandId = appleId, IsFeatured = true, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/macair/300/300" } ] },
                new() { Name = "Samsung S24 Ultra", Price = 1199m, StockQuantity = 30, CategoryId = elecId, BrandId = samsungId, IsFeatured = true, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/s24/300/300" } ] },
                new() { Name = "Nike Zoom Fly 5", Price = 160m, StockQuantity = 20, CategoryId = clothId, BrandId = nikeId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/zoomfly/300/300" } ] },
                new() { Name = "Adidas Track Jacket", Price = 80m, StockQuantity = 55, CategoryId = clothId, BrandId = adidasId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/track/300/300" } ] },
                new() { Name = "Apple Magic Mouse", Price = 79m, StockQuantity = 90, CategoryId = elecId, BrandId = appleId, IsFeatured = false, Images = [ new() { IsMain = true, ImageUrl = "https://picsum.photos/seed/mouse/300/300" } ] }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}