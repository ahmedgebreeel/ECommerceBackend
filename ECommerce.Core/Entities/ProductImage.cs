namespace ECommerce.Core.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }

        // Parent -> Child(ProductImage)

        //One to Many relation with Product ( Product (1) -> (N) ProductImage )
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
