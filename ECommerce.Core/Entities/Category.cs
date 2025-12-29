namespace ECommerce.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string HierarchyPath { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //Parent -> Child(Category)

        //One to Many Relation with Category ( Category (1) -> (N) Category )
        public int? ParentId { get; set; }
        public virtual Category? Parent { get; set; }

        //Parent(Category) -> Child

        //One To Many Relation With Category ( Category (1) -> (N) Category )
        public virtual ICollection<Category> SubCategories { get; set; } = [];

        //One to Many Relation With Product ( Category (1) -> (N) Product )
        public virtual ICollection<Product> Products { get; set; } = [];


    }
}
