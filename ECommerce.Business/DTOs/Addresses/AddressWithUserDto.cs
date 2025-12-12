namespace ECommerce.Business.DTOs.Addresses
{
    public class AddressWithUserDto : AddressDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
