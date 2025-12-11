namespace ECommerce.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        //many to one relation with Order
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        //many to one relation with Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

    }
}
