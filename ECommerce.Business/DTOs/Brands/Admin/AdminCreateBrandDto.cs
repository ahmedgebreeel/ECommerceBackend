using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Brands.Admin
{
    public class AdminCreateBrandDto
    {
        [Required(ErrorMessage = "Brand Name is required")]
        [MaxLength(50, ErrorMessage = "Brand Name cannot exceed 50 characters")]
        public string Name { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }
}
