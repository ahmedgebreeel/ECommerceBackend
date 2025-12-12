namespace ECommerce.Core.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        //one to many relation with Product
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}
