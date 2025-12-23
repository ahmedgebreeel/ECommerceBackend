namespace ECommerce.Business.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string HierarchyPath { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }
}
