using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.Orders.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Orders;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
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

        #region Admin Dashboard
        public async Task<PagedResponseDto<AdminOrderDto>> GetAllOrdersAdminAsync(AdminOrderSpecParams specParams)
        {
            var query = _context.Orders.AsNoTracking().Include(o => o.User).AsQueryable();


            //Filter
            if (specParams.Status.HasValue)
            {
                query = query.Where(o => o.Status == specParams.Status.Value);
            }

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                var term = specParams.Search.Trim().ToLower();
                bool isNumeric = int.TryParse(term, out int searchId);

                query = query.Where(o => (isNumeric && o.Id == searchId) ||
                                    o.User.FirstName.Contains(term) ||
                                    o.User.LastName.Contains(term));
            }

            //Sort
            query = specParams.Sort switch
            {
                "oldest" => query.OrderBy(o => o.Created),
                "highestTotal" => query.OrderByDescending(o => o.TotalAmount),
                "lowestTotal" => query.OrderBy(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.Created)
            };

            //Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<AdminOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponseDto<AdminOrderDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<AdminOrderDetailsDto> GetOrderDetailsAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .ProjectTo<AdminOrderDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");
            return order;
        }

        public async Task<AdminOrderDetailsDto> UpdateOrderAdminAsync(int orderId, AdminUpdateOrderDto dto)
        {


            var orderToUpdate = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new NotFoundException("Order does not exist.");

            var changes = new List<string>();
            var oldStatus = orderToUpdate.Status;

            //FastExit if status is same and no addressId sent
            if (orderToUpdate.Status == dto.Status && dto.AddressId is null)
            {
                return _mapper.Map<AdminOrderDetailsDto>(orderToUpdate);
            }

            //update status if not same
            if (orderToUpdate.Status != dto.Status)
            {
                //block terminal status updates
                if (orderToUpdate.Status == OrderStatus.Delivered || orderToUpdate.Status == OrderStatus.Cancelled)
                    throw new BadRequestException($"Order is {orderToUpdate.Status}. No further changes allowed.");

                //Shipped Orders can only be updated to Delivered
                if (orderToUpdate.Status == OrderStatus.Shipped && dto.Status != OrderStatus.Delivered)
                    throw new BadRequestException($"Order is {orderToUpdate.Status}. Can only be updated to delivered.");

                // Prevent Teleporting: Must be Shipped before Delivered
                if (dto.Status == OrderStatus.Delivered && orderToUpdate.Status != OrderStatus.Shipped)
                    throw new BadRequestException("Order must be marked as Shipped before it can be Delivered.");

                //if order is pending or processing and new status is canceled => retrieve stock
                if ((orderToUpdate.Status == OrderStatus.Pending || orderToUpdate.Status == OrderStatus.Processing) && dto.Status == OrderStatus.Cancelled)
                {
                    foreach (var item in orderToUpdate.Items)
                    {
                        if (item.Product is not null)
                            item.Product.StockQuantity += item.Quantity;
                    }
                    changes.Add("Inventory restocked");
                }

                orderToUpdate.Status = dto.Status;
                changes.Add($"Status: {oldStatus} -> {dto.Status}");
            }

            //update address if order status is pending or processing & new addressid is not null
            if ((orderToUpdate.Status == OrderStatus.Pending || orderToUpdate.Status == OrderStatus.Processing) && dto.AddressId is not null)
            {
                var address = await _context.Addresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == dto.AddressId && a.UserId == orderToUpdate.UserId)
                    ?? throw new NotFoundException("Address does not exist or belongs to another user.");
                orderToUpdate.ShippingAddress = new OrderAddress
                {
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                };
                changes.Add($"Shipping Address updated (Source ID: {dto.AddressId})");
            }
            var changesWrittenToDB = await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information) && changes.Count > 0 && changesWrittenToDB > 0)
            {
                var changeSummary = string.Join(", ", changes);
                _logger.LogInformation("Order {OrderId} updated by Admin. Changes: {ChangeSummary}", orderToUpdate.Id, changeSummary);
            }
            return _mapper.Map<AdminOrderDetailsDto>(orderToUpdate);

        }

        public async Task DeleteOrderAdminAsync(int orderId)
        {

            var orderToDelete = await _context.Orders.FindAsync(orderId)
                ?? throw new NotFoundException("Order does not exist.");

            _context.Orders.Remove(orderToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order deleted with id = {orderId}.", orderToDelete.Id);
        }

        #endregion

        #region Customer

        public async Task<PagedResponseDto<OrderDto>> GetAllOrdersAsync(OrderSpecParams specParams)
        {
            var currentUserId = GetCurrentUserId();

            var query = _context.Orders.AsNoTracking().Where(o => o.UserId == currentUserId).AsQueryable();

            //Filter
            if (specParams.Status is not null)
            {
                query = query.Where(o => o.Status == specParams.Status);
            }

            //Sort (oldest,totalAsc,totalDesc)
            query = specParams.Sort switch
            {
                "oldest" => query.OrderBy(o => o.Created),
                "totalAsc" => query.OrderBy(o => o.TotalAmount),
                "totalDesc" => query.OrderByDescending(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.Created)
            };

            //Paginate
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .AsSplitQuery()
                .ToListAsync();

            return new PagedResponseDto<OrderDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };

        }

        public async Task<OrderDto> CheckoutAsync(CheckoutDto dto)
        {
            //get current user
            var currentUserId = GetCurrentUserId();

            // get User's Cart (Include Product info for Price/Stock validation)
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId);

            if (cart == null || cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");

            //get shipping address of user
            var shippingAddress = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == dto.ShippingAddressId && a.UserId == currentUserId)
                .ProjectTo<OrderAddress>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Shipping Address not found.");

            //create order
            var orderToCreate = new Order
            {
                UserId = currentUserId,
                Status = OrderStatus.Pending,
                ShippingFees = dto.ShippingMethod switch
                {
                    ShippingMethod.Standard => 150,
                    ShippingMethod.Express => 250,
                    _ => 150
                },
                ShippingMethod = dto.ShippingMethod,
                ShippingAddress = shippingAddress,
                Items = [],
                OrderTrackingMilestones = []
            };
            //Loop over the cart items => validate stock => add them to order
            foreach (var cartItem in cart.Items)
            {
                //product stock check
                if (cartItem.Quantity > cartItem.Product.StockQuantity)
                    throw new BadRequestException($"Not enough stock for {cartItem.Product.Name}. Available: {cartItem.Product.StockQuantity}");
                cartItem.Product.StockQuantity -= cartItem.Quantity;

                orderToCreate.Items.Add(_mapper.Map<OrderItem>(cartItem));
            }
            orderToCreate.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
            });
            orderToCreate.Subtotal = orderToCreate.Items.Sum(i => i.TotalPrice);
            orderToCreate.Taxes = 0.14m * orderToCreate.Subtotal;
            orderToCreate.TotalAmount = orderToCreate.Subtotal + orderToCreate.ShippingFees + orderToCreate.Taxes;

            _context.Orders.Add(orderToCreate);
            _context.CartItems.RemoveRange(cart.Items);
            cart.LastUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Stock changed during checkout. Please try again.");
            }

            return _mapper.Map<OrderDto>(orderToCreate);

        }

        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        #endregion  

    }
}
