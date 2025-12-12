namespace ECommerce.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public bool IsFeatured { get; set; }
        public byte[] Version { get; set; } = [];

        //one to many Relationship with ProductImage
        public virtual ICollection<ProductImage> Images { get; set; } = [];

        //one to many Relationship with OrderItems
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        //many to one relation with Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        //many to one relation with Brand
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;

        //one to many relation with CartItems
        public virtual ICollection<CartItem> CartItems { get; set; } = [];

    }
}
