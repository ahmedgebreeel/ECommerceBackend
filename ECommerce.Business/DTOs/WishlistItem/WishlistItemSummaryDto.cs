namespace ECommerce.Business.DTOs.WishlistItem
{
    public class WishlistItemSummaryDto
    {
        public int ProductId { get; set; }
        public string ProductThumbnailUrl { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public bool InStock { get; set; }
        public bool IsDeleted { get; set; }

    }
}
