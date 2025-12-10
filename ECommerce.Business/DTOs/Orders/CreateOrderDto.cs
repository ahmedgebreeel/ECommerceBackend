using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.Orders
{
    public class CreateOrderDto
    {
        public ICollection<CreateOrderItemDto> Items { get; set; } = null!;

    }
}
