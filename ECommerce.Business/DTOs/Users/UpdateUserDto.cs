using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Users
{
    public class UpdateUserDto // User Updates his account details
    {
        public string UserName { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
