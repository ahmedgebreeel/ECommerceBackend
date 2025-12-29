namespace ECommerce.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsDeleted { get; set; } = false;
        public byte[] Version { get; set; } = [];

        //Parent -> Product(Child)

        //One to Many relation with Category ( Category (1) -> (N) Porduct ) 
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        //One to Many relation with Brand ( Brand (1) -> (N) Porduct ) 
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;

        //Product(Parent) -> Child

        //One to Many Relationship with ProductImage ( Product (1) -> (N) ProductImage ) 
        public virtual ICollection<ProductImage> Images { get; set; } = [];

        //One to Many relation with CartItems ( Product (1) -> (N) CartItem ) 
        public virtual ICollection<CartItem> CartItems { get; set; } = [];

        //One To Many Relation with WishlistItem ( Product (1) -> (N) WishListItem )
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = [];

    }
}
