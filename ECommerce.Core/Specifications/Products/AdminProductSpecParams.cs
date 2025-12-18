namespace ECommerce.Core.Specifications.Products
{
    public class AdminProductSpecParams
    {
        //Filter
        public string? Status { get; set; }

        //Search Params
        public string? Search { get; set; }

        //Pagination Params
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 50) ? 50 : value;
        }
    }
}
