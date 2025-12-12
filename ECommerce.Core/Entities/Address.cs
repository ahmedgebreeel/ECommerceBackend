namespace ECommerce.Core.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string? PostalCode { get; set; }
        public string Country { get; set; } = null!;

        //many to one relation with user
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
