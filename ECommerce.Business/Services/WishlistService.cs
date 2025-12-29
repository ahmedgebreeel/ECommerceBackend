using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.WishlistItem;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class WishlistService(AppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext) : IWishlistService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        public async Task<IEnumerable<WishlistItemSummaryDto>> GetWishlistAsync()
        {
            var currentUserId = GetCurrentUserId();
            var wishlist = await _context.WishlistItems
                .Where(wi => wi.Wishlist.UserId == currentUserId)
                .IgnoreQueryFilters()
                .ProjectTo<WishlistItemSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (wishlist == null)
            {
                return [];
            }
            return wishlist;

        }

        public async Task<int> AddToWishlistAsync(int productId)
        {
            var currentUserId = GetCurrentUserId();

            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                throw new NotFoundException("Product does not exist");

            var wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = currentUserId
                };
                _context.Wishlists.Add(wishlist);
            }
            //product already in wishlist
            if (wishlist.Items.Select(wi => wi.ProductId).Contains(productId))
                return productId;

            wishlist.Items.Add(new WishlistItem
            {
                ProductId = productId,
            });

            await _context.SaveChangesAsync();
            return productId;

        }

        public async Task RemoveFromWishlistAsync(int productId)
        {
            var currentUserId = GetCurrentUserId();
            var wishlist = await _context.Wishlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);

            if (wishlist == null)
            {
                return;
            }

            if (!wishlist.Items.Select(wi => wi.ProductId).Contains(productId))
                return;

            var wishlistItemToRemove = wishlist.Items
                .Where(wi => wi.ProductId == productId)
                .FirstOrDefault()
                ?? throw new NotFoundException("Produdct does not exist in wishlist.");

            wishlist.Items.Remove(wishlistItemToRemove);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<int>> GetWishlistIdsAsync()
        {
            var currentUserId = GetCurrentUserId();
            return await _context.WishlistItems
                .Where(wi => wi.Wishlist.UserId == currentUserId)
                .Select(wi => wi.ProductId)
                .ToListAsync();
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
