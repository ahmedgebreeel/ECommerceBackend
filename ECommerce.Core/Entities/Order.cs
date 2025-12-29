using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        //Owned Entity (Snapshot)
        public OrderAddress ShippingAddress { get; set; } = null!;

        //Parent -> Child(Order

        //One to Many Relation With ApplicationUser ( ApplicationUser (1) -> (N) Order )
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        //Parent(Order) -> Child

        //One to Many Relation with OrderItem ( Order (1) -> (N) OrderItem )
        public virtual ICollection<OrderItem> Items { get; set; } = [];

        //One to Many Relation With OrderTrackingMilestone ( Order (1) -> (N) OrderTrackingMilestone )
        public virtual ICollection<OrderTrackingMilestone> OrderTrackingMilestones { get; set; } = [];
    }
}
