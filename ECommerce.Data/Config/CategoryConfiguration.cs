using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(50);


            builder.Property(x => x.Description)
                .HasColumnType("VARCHAR(MAX)");

            //one to many relation with Product
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            //Seed Data
            builder.HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Men and women clothing" },
                new Category { Id = 3, Name = "Books", Description = "Fiction, non-fiction, academic" },
                new Category { Id = 4, Name = "Home", Description = "Furniture & home accessories" }
            );
        }
    }
}
