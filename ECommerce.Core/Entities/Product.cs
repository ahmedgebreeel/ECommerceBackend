namespace ECommerce.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public byte[] Version { get; set; } = null!;

        //one to many Relationship with ProductImage
        public virtual ICollection<ProductImage> Images { get; set; } = null!;

        //one to many Relationship with OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;

        //many to one relation with Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        //many to one relation with Brand
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;

    }
}
