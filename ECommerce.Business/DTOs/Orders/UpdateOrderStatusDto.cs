using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
