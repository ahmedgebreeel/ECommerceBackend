namespace ECommerce.Core.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public bool IsDefault { get; set; }

        //Personal Info
        public string FullName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;

        //Address Info
        public string Label { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Building { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? ZipCode { get; set; } = null!;
        public string? Hints { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //Parent -> Child(Address)

        //One to Many Relation with ApplicationUser ( ApplicationUser (1) -> (N) Address )
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
