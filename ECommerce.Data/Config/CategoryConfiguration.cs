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
                .HasColumnType("NVARCHAR(50)");


            builder.Property(x => x.Description)
                .HasColumnType("NVARCHAR(1000)");

            builder.Property(c => c.HierarchyPath)
                .HasColumnType("NVARCHAR(500)");

            //One To Many Relation With Category ( Category (1) -> (N) Category )
            builder.HasMany(c => c.SubCategories)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            //One to Many Relation With Product ( Category (1) -> (N) Product )
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
