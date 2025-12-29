using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class OrderTrackingMilestone
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime TimeStamp { get; set; }

        //Parent -> Child(OrderTrackingMilestone)

        //One to Many Relation with Order ( Order (1) -> (N) OrderTrackingMilestone )
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
