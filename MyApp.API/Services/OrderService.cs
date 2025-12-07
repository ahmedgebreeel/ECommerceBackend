using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Orders;
using MyApp.API.Entities;
using MyApp.API.Enums;
using MyApp.API.Exceptions;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class OrderService(AppDbContext context, IMapper mapper, ILogger<OrderService> logger) : IOrderService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return orders;
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _context.Orders.Where(o => o.Id == id)
                .AsNoTracking()
                 .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync()
                 ?? throw new NotFoundException("Order does not exist.");
            return order;

        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new BadRequestException("Order must contain at least one item.");

            var orderToCreate = new Order
            {
                Created = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = []
            };

            foreach (var dtoItem in dto.Items)
            {
                var product = await _context.Products.FindAsync(dtoItem.ProductId)
                    ?? throw new NotFoundException($"Product does not exist.");

                if (dtoItem.Quantity <= 0)
                    throw new BadRequestException($"Invalid quantity for product {product.Name}");

                if (product.StockQuantity < dtoItem.Quantity)
                    throw new BadRequestException($"Not enough stock for product {product.Name}");

                // Reduce stock
                product.StockQuantity -= dtoItem.Quantity;

                // Create order item
                var item = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = dtoItem.Quantity,
                    UnitPrice = product.Price
                };

                // Use computed TotalPrice
                orderToCreate.TotalAmount += item.TotalPrice;

                orderToCreate.Items.Add(item);
            }

            // EF will generate OrderId and apply it to children automatically
            _context.Orders.Add(orderToCreate);

            // Save everything at once → atomic transaction
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order added with id = {orderId}.", orderToCreate.Id);

            return _mapper.Map<OrderDto>(orderToCreate);
        }

        public async Task<OrderDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var orderToUpdate = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new NotFoundException("Order does not exist.");

            orderToUpdate.Status = dto.Status;
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order updated with id = {orderId}.", orderToUpdate.Id);
            return _mapper.Map<OrderDto>(orderToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var orderToDelete = await _context.Orders.FindAsync(id)
                ?? throw new NotFoundException("Invalid OrderId");
            _context.Orders.Remove(orderToDelete);
            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order deleted with id = {orderId}.", orderToDelete.Id);
        }
    }
}
