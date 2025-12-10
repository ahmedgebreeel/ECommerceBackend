using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        //public int UserId { get; set; }
        //public virtual User User { get; set; }
        public DateTime Created { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        //one to many relation with OrderItems
        public virtual ICollection<OrderItem> Items { get; set; } = null!;

        //many to one relation with User
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
