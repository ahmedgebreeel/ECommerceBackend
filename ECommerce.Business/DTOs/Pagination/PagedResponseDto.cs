namespace ECommerce.Business.DTOs.Pagination
{
    public class PagedResponseDto<T>()
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get
            {
                if (PageSize <= 0) return 0;
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            }
        }
        public string? Sort { get; set; }
        public string? Search { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public IEnumerable<T> Items { get; set; } = [];
    }
}
