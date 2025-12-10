using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero.")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to zero.")]
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }


}
