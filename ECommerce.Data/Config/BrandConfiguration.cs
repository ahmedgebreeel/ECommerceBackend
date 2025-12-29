using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {

            builder.Property(x => x.Name)
                .HasColumnType("NVARCHAR(50)");


            builder.Property(x => x.Description)
                .HasColumnType("NVARCHAR(1000)");


            //One to Many Relation with Product ( Brand (1) -> (N) Product)
            builder.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
