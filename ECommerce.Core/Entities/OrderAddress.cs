using Microsoft.EntityFrameworkCore;

namespace ECommerce.Core.Entities
{
    [Owned]
    public class OrderAddress
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string? PostalCode { get; set; }
        public string Country { get; set; } = null!;
    }
}
