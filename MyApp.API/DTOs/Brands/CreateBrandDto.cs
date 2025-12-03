namespace MyApp.API.DTOs.Brands
{
    public class CreateBrandDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
