namespace ECommerce.Business.DTOs.Auth.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        //User Information
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = [];
        public string? AvatarUrl { get; set; }
    }
}
