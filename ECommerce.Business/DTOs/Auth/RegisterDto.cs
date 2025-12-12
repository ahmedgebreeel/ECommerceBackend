using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        [Phone]
        public string? PhoneNumber { get; set; }

    }
}
