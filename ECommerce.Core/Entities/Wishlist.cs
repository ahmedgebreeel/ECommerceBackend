namespace ECommerce.Core.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public DateTime Updated { get; set; }

        // Parent -> Child (Wishlist)

        //One to One relation with ApplicationUser ( ApplicationUser (1) -> (1) Wishlist )
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        //Parent(Wishlist) -> Child

        //One to Many relation with WishlistItem ( Wishlist (1) -> (N) WishlistItem )
        public List<WishlistItem> Items { get; set; } = [];
    }
}
