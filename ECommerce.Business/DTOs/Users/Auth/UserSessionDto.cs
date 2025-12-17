namespace ECommerce.Business.DTOs.Users.Auth
{
    public class UserSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
