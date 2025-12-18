using Bogus;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Seeders
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Orders.AnyAsync()) return;

            // 1. UPDATE: Include Images so we can snapshot the PictureUrl
            var products = await context.Products
                .Include(p => p.Images) // <--- CRITICAL for snapshotting
                .ToListAsync();

            var customers = await userManager.GetUsersInRoleAsync("Customer");

            if (products.Count == 0 || customers.Count == 0) return;

            var faker = new Faker();
            var orders = new List<Order>();

            for (int i = 0; i < 50; i++)
            {
                var user = faker.PickRandom(customers);
                var orderProducts = faker.PickRandom(products, faker.Random.Int(1, 5)).ToList();

                var order = CreateOrder(user, orderProducts, faker);
                orders.Add(order);
            }

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }

        private static Order CreateOrder(ApplicationUser user, List<Product> products, Faker faker)
        {
            var status = faker.Random.WeightedRandom(
                new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Cancelled },
                new[] { 0.1f, 0.15f, 0.2f, 0.45f, 0.1f }
            );

            var createdDate = faker.Date.Past(3);

            var shippingMethod = faker.PickRandom<ShippingMethod>();
            decimal shippingFees = shippingMethod == ShippingMethod.Express ? 250m : 150m;

            var shippingAddress = new OrderAddress
            {
                Street = faker.Address.StreetAddress(),
                City = faker.Address.City(),
                State = faker.Address.State(),
                PostalCode = faker.Address.ZipCode(),
                Country = "Egypt"
            };

            // --- C. Build Order Items (UPDATED) ---
            var orderItems = new List<OrderItem>();
            foreach (var p in products)
            {
                orderItems.Add(new OrderItem
                {
                    // 1. The Link (For constraints/analytics)
                    ProductId = p.Id,

                    // 2. Transaction Data
                    Quantity = faker.Random.Int(1, 3),
                    UnitPrice = p.Price,

                    // 3. THE SNAPSHOT (New Value Object)
                    ProductOrdered = new ProductItemOrdered
                    {
                        ProductId = p.Id, // Original ID
                        ProductName = p.Name,
                        PictureUrl = p.Images.FirstOrDefault(x => x.IsMain)?.ImageUrl
                    }
                });
            }

            var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
            var taxes = subtotal * 0.14m;
            var totalAmount = subtotal + taxes + shippingFees;

            var order = new Order
            {
                UserId = user.Id,
                Created = createdDate,
                Updated = createdDate,
                Status = status,
                ShippingMethod = shippingMethod,
                ShippingAddress = shippingAddress,
                Subtotal = subtotal,
                ShippingFees = shippingFees,
                Taxes = taxes,
                TotalAmount = totalAmount,
                Items = orderItems,
                OrderTrackingMilestones = []
            };

            // --- Milestones Logic (Same as before) ---
            order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
                TimeStamp = createdDate
            });

            if (status == OrderStatus.Cancelled)
            {
                var cancelDate = createdDate.AddHours(faker.Random.Double(1, 24));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Cancelled,
                    TimeStamp = cancelDate
                });
                order.Updated = cancelDate;
                return order;
            }

            if (status >= OrderStatus.Processing)
            {
                var processDate = createdDate.AddHours(faker.Random.Double(2, 5));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Processing,
                    TimeStamp = processDate
                });
                order.Updated = processDate;

                if (status >= OrderStatus.Shipped)
                {
                    var shipDate = processDate.AddDays(faker.Random.Double(1, 2));
                    order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                    {
                        Status = OrderStatus.Shipped,
                        TimeStamp = shipDate
                    });
                    order.Updated = shipDate;

                    if (status == OrderStatus.Delivered)
                    {
                        var deliverDate = shipDate.AddDays(faker.Random.Double(2, 5));
                        order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                        {
                            Status = OrderStatus.Delivered,
                            TimeStamp = deliverDate
                        });
                        order.Updated = deliverDate;
                    }
                }
            }

            return order;
        }
    }
}