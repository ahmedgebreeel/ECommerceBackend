using ECommerce.Business.DTOs.WishlistItem;

namespace ECommerce.Business.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemSummaryDto>> GetWishlistAsync();
        Task<int> AddToWishlistAsync(int productId);
        Task RemoveFromWishlistAsync(int productId);
        Task<IEnumerable<int>> GetWishlistIdsAsync();

    }
}
