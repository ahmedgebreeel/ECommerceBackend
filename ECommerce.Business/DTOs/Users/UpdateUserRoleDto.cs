namespace ECommerce.Business.DTOs.Users
{
    public class UpdateUserRoleDto
    {
        public int UserId { get; set; }
        public string Role { get; set; } = null!;
    }
}
