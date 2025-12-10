using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = null!;


    }
}
