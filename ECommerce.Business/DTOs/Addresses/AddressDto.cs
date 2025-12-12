namespace ECommerce.Business.DTOs.Addresses
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}
