using ECommerce.Business.DTOs.Orders;

namespace ECommerce.Business.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto);
        Task DeleteAsync(int id); // Admin
    }
}
