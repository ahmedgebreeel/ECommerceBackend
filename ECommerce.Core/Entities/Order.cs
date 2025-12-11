using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;

        //one to many relation with OrderItems
        public virtual ICollection<OrderItem> Items { get; set; } = null!;

        //many to one relation with User
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
