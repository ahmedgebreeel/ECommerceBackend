namespace ECommerce.Business.DTOs.Brands.Admin
{
    public class AdminBrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductsCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
