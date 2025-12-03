using MyApp.API.Enums;

namespace MyApp.API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        //many to one relation with User
        //public int UserId { get; set; }
        //public virtual User User { get; set; }
        public DateTime Created { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        //one to many relation with OrderItems
        public virtual ICollection<OrderItem> Items { get; set; } = null!;

        public Order()
        {
            Created = DateTime.UtcNow;
        }
    }
}
