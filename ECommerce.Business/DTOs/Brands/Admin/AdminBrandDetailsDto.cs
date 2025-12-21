using ECommerce.Business.DTOs.Products.Admin;

namespace ECommerce.Business.DTOs.Brands.Admin
{
    public class AdminBrandDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<AdminProductDto> Products { get; set; } = [];
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
