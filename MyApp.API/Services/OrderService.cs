using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Orders;
using MyApp.API.Entities;
using MyApp.API.Enums;
using MyApp.API.Exceptions;
using MyApp.API.Interfaces;
using System.Security.Claims;

namespace MyApp.API.Services
{
    public class OrderService(
        AppDbContext context,
        IMapper mapper,
        ILogger<OrderService> logger,
        IHttpContextAccessor httpContext
        ) : IOrderService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var currentUserId = GetCurrentUserId();
            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == currentUserId)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return orders;
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == currentUserId && o.Id == id)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");
            return order;

        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new BadRequestException("Order must contain at least one item.");

            var currentUserId = GetCurrentUserId();
            var orderToCreate = new Order
            {
                UserId = currentUserId,
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

                product.StockQuantity -= dtoItem.Quantity;

                var item = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = dtoItem.Quantity,
                    UnitPrice = product.Price
                };

                orderToCreate.TotalAmount += item.TotalPrice;

                orderToCreate.Items.Add(item);
            }

            // EF will generate OrderId and apply it to children automatically
            _context.Orders.Add(orderToCreate);
            await _context.SaveChangesAsync();


            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order added with id = {orderId} associated with user with id = {userId}.",
                    orderToCreate.Id,
                    orderToCreate.UserId);

            return _mapper.Map<OrderDto>(orderToCreate);
        }

        public async Task<OrderDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var orderToUpdate = await _context.Orders
                .Where(o => o.Id == id && o.UserId == currentUserId)
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
            var currentUserId = GetCurrentUserId();
            var orderToDelete = await _context.Orders
                .Where(o => o.Id == id && o.UserId == currentUserId)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");

            _context.Orders.Remove(orderToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order deleted with id = {orderId}.", orderToDelete.Id);
        }

    }
}
