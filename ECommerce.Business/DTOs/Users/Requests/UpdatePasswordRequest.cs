namespace ECommerce.Business.DTOs.Users.Requests
{
    public class UpdatePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
