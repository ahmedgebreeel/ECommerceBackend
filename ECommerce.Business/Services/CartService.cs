using AutoMapper;
using ECommerce.Business.DTOs.ShoppingCart.Requests;
using ECommerce.Business.DTOs.ShoppingCart.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class CartService(
        AppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext,
        ILogger<CartService> logger) : ICartService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private readonly ILogger<CartService> _logger = logger;

        public async Task<CartResponse> GetCartAsync()
        {
            //fetch current user cart.
            var currentUserId = GetCurrentUserId();

            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId);

            //initialize cart for new users.
            if (cart == null)
            {
                return new CartResponse();
            }

            //check soft deleted products.
            var invalidItems = cart.Items
                .Where(i => i.Product.IsDeleted)
                .ToList();

            List<string> warnings = [];

            if (invalidItems.Count > 0)
            {
                //remove from Databse
                _context.CartItems.RemoveRange(invalidItems);

                // Remove from memory so the mapped DTO is clean
                foreach (var item in invalidItems)
                {
                    cart.Items.Remove(item);
                    warnings.Add($"Item '{item.Product.Name}' got removed because it is no longer available.");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} deleted products from user cart.", invalidItems.Count);
            }

            var response = _mapper.Map<CartResponse>(cart);
            response.Warnings = warnings;
            return response;

        }

        public async Task<CartResponse> UpdateCartAsync(UpdateCartRequest updateCartRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //Get current user cart
                var currentUserId = GetCurrentUserId();
                var cart = await _context.ShoppingCarts
                    .Include(c => c.Items)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(sc => sc.UserId == currentUserId);
                //initialize cart for new users
                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = currentUserId };
                    _context.ShoppingCarts.Add(cart);
                }
                //delete items from Database not included in the update request 
                var incomingProductIds = updateCartRequest.Items.Select(i => i.ProductId).Distinct().ToList();

                if (cart.Id > 0)
                {
                    await _context.CartItems
                        .Where(i => i.ShoppingCartId == cart.Id && !incomingProductIds.Contains(i.ProductId))
                        .ExecuteDeleteAsync();
                }

                cart.Items.RemoveAll(i => !incomingProductIds.Contains(i.ProductId));

                //update cart
                var productsDict = await _context.Products
                    .Include(p => p.Images)
                    .Where(p => incomingProductIds.Contains(p.Id))
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(p => p.Id);

                List<string> warnings = [];

                foreach (var itemDto in updateCartRequest.Items)
                {
                    //product does not exist or hard deleted(redundant as the database restricts deleting products with cart items)
                    if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                    {
                        warnings.Add($"Item {itemDto.ProductId} not added because it is no longer exists.");
                        continue;
                    }
                    //soft deleted product
                    if (product.IsDeleted)
                    {
                        warnings.Add($"Item '{product.Name}' not added because it is no longer available.");
                        continue;
                    }
                    //quantity of item in request exceeds the available product stock
                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        itemDto.Quantity = product.StockQuantity;
                        warnings.Add($"Quantity for '{product.Name}' adjusted to {product.StockQuantity} due to stock limits.");
                    }
                    //update item if existing in cart or initialize it if not.
                    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity = itemDto.Quantity;
                        existingItem.Product = product;
                    }
                    else
                    {
                        cart.Items.Add(new CartItem
                        {
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            Product = product
                        });
                    }
                }
                //update cart metadata
                cart.Updated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = _mapper.Map<CartResponse>(cart);
                response.Warnings = warnings;
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ClearCartAsync()
        {
            var currentUserId = GetCurrentUserId();

            await _context.CartItems
                .Where(i => i.ShoppingCart.UserId == currentUserId)
                .ExecuteDeleteAsync();

            await _context.ShoppingCarts
                .Where(sc => sc.UserId == currentUserId)
                .ExecuteUpdateAsync(x => x.SetProperty(sc => sc.Updated, DateTime.UtcNow));
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }
    }
}
