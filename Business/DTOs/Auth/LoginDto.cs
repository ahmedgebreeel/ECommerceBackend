namespace ECommerce.Business.DTOs.Auth
{
    public class LoginDto
    {
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
