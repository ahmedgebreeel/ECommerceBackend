namespace ECommerce.Core.Specifications.Products
{
    public class ProductSpecParams
    {
        //Filter Params
        public List<int> BrandsIdsList = [];
        public string? BrandsIds
        {
            get => string.Join(",", BrandsIdsList);

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    BrandsIdsList = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList();
                }
            }
        }

        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        //Search Params
        public string? Search { get; set; }

        //Sort Params
        public string? Sort { get; set; }

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
