namespace MyApp.API.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        //one to many relation with Product
        public virtual ICollection<Product> Products { get; set; } = null!;

    }
}
