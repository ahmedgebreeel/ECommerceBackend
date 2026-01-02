namespace ECommerce.Core.Specifications.Users
{
    public class AdminUserSpecParams
    {
        //Filter Pararms
        public string? Role { get; set; }
        public string? Status { get; set; }

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
