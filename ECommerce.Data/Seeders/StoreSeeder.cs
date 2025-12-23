using Bogus;
using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Seeders
{
    public static class StoreSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedBrandsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedBrandsAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            var brands = new List<Brand>
            {
                new() { Name = "Nike", Description = "Just Do It" },
                new() { Name = "Adidas", Description = "Impossible is Nothing" },
                new() { Name = "Zara", Description = "Fast Fashion & Trendy" },
                new() { Name = "H&M", Description = "Sustainable Fashion" },
                new() { Name = "Levi's", Description = "Quality Denim" },
                new() { Name = "Puma", Description = "Forever Faster" },
                new() { Name = "Gucci", Description = "Luxury Fashion" },
                new() { Name = "Uniqlo", Description = "Modern Essentials" },
                new() { Name = "Ralph Lauren", Description = "Premium Lifestyle" },
                new() { Name = "Calvin Klein", Description = "Modern Minimalist" }
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>();

            // Define Roots
            var men = new Category { Name = "Men", HierarchyPath = "Men" };
            var women = new Category { Name = "Women", HierarchyPath = "Women" };
            var kids = new Category { Name = "Kids", HierarchyPath = "Kids" };

            categories.AddRange(new[] { men, women, kids });

            // --- MEN ---
            var mClothing = CreateChild(men, "Men's Clothing");
            var mFootwear = CreateChild(men, "Men's Footwear");
            var mAccess = CreateChild(men, "Men's Accessories");
            categories.AddRange(new[] { mClothing, mFootwear, mAccess });

            // Men Leaves
            categories.Add(CreateChild(mClothing, "Men's T-Shirts"));
            categories.Add(CreateChild(mClothing, "Men's Jeans"));
            categories.Add(CreateChild(mFootwear, "Men's Sneakers"));
            categories.Add(CreateChild(mFootwear, "Men's Formal Shoes"));
            categories.Add(CreateChild(mAccess, "Men's Watches"));

            // --- WOMEN ---
            var wClothing = CreateChild(women, "Women's Clothing");
            var wFootwear = CreateChild(women, "Women's Footwear");
            var wAccess = CreateChild(women, "Women's Accessories");
            categories.AddRange(new[] { wClothing, wFootwear, wAccess });

            // Women Leaves
            categories.Add(CreateChild(wClothing, "Women's Dresses"));
            categories.Add(CreateChild(wClothing, "Women's Tops"));
            categories.Add(CreateChild(wFootwear, "Women's Heels"));
            categories.Add(CreateChild(wAccess, "Women's Handbags"));

            // --- KIDS ---
            var kClothing = CreateChild(kids, "Kids' Clothing");
            var kFootwear = CreateChild(kids, "Kids' Footwear");
            categories.AddRange(new[] { kClothing, kFootwear });

            categories.Add(CreateChild(kClothing, "Kids' Baby Wear"));
            categories.Add(CreateChild(kFootwear, "Kids' School Shoes"));

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Helper Function
        private static Category CreateChild(Category parent, string name)
        {
            return new Category
            {
                Name = name,
                Parent = parent,
                HierarchyPath = $"{parent.HierarchyPath}\\{name}"
            };
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var brands = await context.Brands.ToListAsync();
            var categories = await context.Categories.ToListAsync();
            var faker = new Faker();

            var products = new List<Product>();

            // The Blueprint Logic:
            // Maps a specific "Leaf Category Name" -> to a list of "Realistic Product Names"
            // This ensures "Dresses" only go into the Dress category, etc.
            var productBlueprints = new Dictionary<string, string[]>
            {
                // --- MEN ---
                { "Men's T-Shirts", new[] { "Essential Crew Neck", "Graphic Print Tee", "Oversized Cotton T-Shirt", "Vintage Logo Tee", "Performance Polo" } },
                { "Men's Jeans", new[] { "Slim Fit Denim", "Straight Leg Jeans", "Ripped Wash Jeans", "Tapered Cargo Pants", "Classic Blue Jeans" } },
                { "Men's Sneakers", new[] { "Air Runner 500", "Court Vision Low", "Ultralight Jogger", "Retro High-Top", "Streetwear Skate Shoe" } },
                { "Men's Formal Shoes", new[] { "Leather Oxford", "Classic Brogues", "Suede Loafers", "Derby Shoes" } },
                { "Men's Watches", new[] { "Chronograph Steel", "Minimalist Leather Watch", "Digital Sport Watch", "Automatic Diver" } },

                // --- WOMEN ---
                { "Women's Dresses", new[] { "Floral Summer Dress", "Elegant Evening Gown", "Casual Midi Dress", "Wrap Dress", "Knitted Sweater Dress" } },
                { "Women's Tops", new[] { "Silk Blouse", "Crop Top", "Linen Button-Up", "Chiffon Tunic" } },
                { "Women's Heels", new[] { "Classic Stilettos", "Block Heel Sandals", "Pointed Toe Pumps", "Ankle Strap Heels" } },
                { "Women's Handbags", new[] { "Leather Tote", "Quilted Crossbody", "Clutch Bag", "Designer Shoulder Bag" } },

                // --- KIDS ---
                { "Kids' Baby Wear", new[] { "Cotton Onesie", "Soft Knit Set", "Animal Print Romper", "Cozy Sleepsuit" } },
                { "Kids' School Shoes", new[] { "Durable Leather Shoes", "Velcro Strap Sneakers", "Black Formal Shoes" } }
            };

            // Loop through our defined blueprints and create products for matching categories
            foreach (var blueprint in productBlueprints)
            {
                // Find category in DB by Name (e.g. "Sneakers" might be under Men or Kids)
                // We use ToList() to handle potential duplicates if names are reused across trees
                var targetCategories = categories.Where(c => c.Name == blueprint.Key).ToList();

                foreach (var category in targetCategories)
                {
                    // Generate between 5 and 8 products for THIS specific category
                    int productCount = faker.Random.Int(5, 8);

                    for (int i = 0; i < productCount; i++)
                    {
                        var brand = faker.PickRandom(brands);
                        var baseName = faker.PickRandom(blueprint.Value);

                        // Construct a realistic name: "Brand + Item + Color"
                        // Ex: "Nike Air Runner 500 Red"
                        var fullName = $"{brand.Name} {baseName} {faker.Commerce.Color()}";

                        // Unique seed logic: 
                        // Using "CategoryId-Index" ensures images are consistent but distinct per product
                        var seedKey = $"{category.Id}-{i}";

                        var productImages = new List<ProductImage>
                        {
                            new() { IsMain = true, ImageUrl = $"https://picsum.photos/seed/{seedKey}-main/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-side/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-back/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-detail/300/300" }
                        };

                        products.Add(new Product
                        {
                            Name = fullName,
                            Description = faker.Commerce.ProductDescription(),
                            Price = decimal.Parse(faker.Commerce.Price(30, 800)),
                            StockQuantity = faker.Random.Int(10, 100),
                            BrandId = brand.Id,
                            CategoryId = category.Id,
                            IsFeatured = faker.Random.Bool(0.1f), // 10% chance of being featured
                            Images = productImages
                        });
                    }
                }
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}