namespace ECommerce.Core.Entities
{
    public class WishlistItem
    {
        public int Id { get; set; }

        //Parent -> Child(WishlistItem)

        //One to Many relation with Wishlist ( Wishlist (1) -> (N) WishlistItem )
        public int WishlistId { get; set; }
        public virtual Wishlist Wishlist { get; set; } = null!;

        //One to Many relation with Product ( Product (1) -> (N) WishlistItem )
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

    }
}
