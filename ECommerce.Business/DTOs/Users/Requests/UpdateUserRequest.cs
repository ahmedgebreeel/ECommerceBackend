using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Users.Requests
{
    public class UpdateUserRequest
    {
        [MaxLength(50, ErrorMessage = "First Name should not exceed 50 characters.")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Last Name should not exceed 50 characters.")]
        public string LastName { get; set; } = null!;

        [MaxLength(30, ErrorMessage = "Username should not exceed 30 characters.")]
        public string UserName { get; set; } = null!;

        [EmailAddress]
        [MaxLength(256, ErrorMessage = "Email address should not exceed 256 characters.")]
        public string Email { get; set; } = null!;

        [Phone]
        [MaxLength(15, ErrorMessage = "Phone Number should not exceed 15 numbers.")]
        public string? PhoneNumber { get; set; }
    }
}
