using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price Must be a positive value")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a positive value")]
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }


}
