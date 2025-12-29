namespace ECommerce.Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //Parent -> Child(CartItem)

        //One to Many relation with ShoppingCart ( ShoppingCart (1) -> (N) CartItem )
        public int ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        // One to Many relation with Product ( Product (1) -> (N) CartItem )
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
