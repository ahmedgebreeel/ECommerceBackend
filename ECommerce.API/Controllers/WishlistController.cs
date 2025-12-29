using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.WishlistItem;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Wishlist Managment")]
    public class WishlistController(IWishlistService wishlist) : ControllerBase
    {
        private readonly IWishlistService _wishlist = wishlist;

        [HttpGet]
        [EndpointSummary("Get all products added to wishlist.")]
        [ProducesResponseType(typeof(IEnumerable<WishlistItemSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetWishlist()
        {
            var wishlist = await _wishlist.GetWishlistAsync();
            return Ok(wishlist);
        }

        [HttpPost("{productId:int}")]
        [EndpointSummary("Adds product to wishlist.")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddToWishlist([FromRoute] int productId)
        {
            var wishlistedProductId = await _wishlist.AddToWishlistAsync(productId);
            return CreatedAtAction(nameof(GetWishlist), wishlistedProductId);
        }

        [HttpDelete("{productId:int}")]
        [EndpointSummary("Removes product from wishlist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveFromWishlist([FromRoute] int productId)
        {
            await _wishlist.RemoveFromWishlistAsync(productId);
            return NoContent();
        }

        [HttpGet("ids")]
        [EndpointSummary("Retrieves all product ids saved into wishlist.")]
        [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetWishlistIds()
        {
            var wishlistedProductIds = await _wishlist.GetWishlistIdsAsync();
            return Ok(wishlistedProductIds);
        }

    }
}
