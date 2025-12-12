namespace ECommerce.Business.DTOs.Users
{
    public class UserManagementDto // Used In Admin => List Users
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = [];
        public int TotalOrders { get; set; }
    }
}
