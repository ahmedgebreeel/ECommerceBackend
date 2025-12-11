using ECommerce.Core.Entities;

namespace ECommerce.Business.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiration { get; set; }

        public UserDto user { get; set; }

        public IList<string> roles { get; set; }
    }
}
