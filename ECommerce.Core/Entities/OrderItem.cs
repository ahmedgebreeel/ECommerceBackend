namespace ECommerce.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        //Owned Entity (Snapshot)
        public OrderedProduct OrderedProduct { get; set; } = null!;

        //Parent -> Child(OrderItem)

        //One to Many Relation With Order ( Order (1) -> (N) OrderItem )
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

    }
}
