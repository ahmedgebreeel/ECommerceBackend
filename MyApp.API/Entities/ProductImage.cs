namespace MyApp.API.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        // Many to One RelationShip with Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }

    }
}
