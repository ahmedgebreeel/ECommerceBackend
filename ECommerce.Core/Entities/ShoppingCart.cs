namespace ECommerce.Core.Entities
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public DateTime Updated { get; set; }

        //Parent -> Child(ShoppingCart)

        //One to One Relation with ApplicationUser ( ApplicationUser (1) -> (1) ShoppingCart )
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        //Parent(ShoppingCart) -> Child

        //One to Many Relation with CartItem ( ShoppingCart (1) -> (N) CartItem )
        public virtual List<CartItem> Items { get; set; } = [];
    }
}
