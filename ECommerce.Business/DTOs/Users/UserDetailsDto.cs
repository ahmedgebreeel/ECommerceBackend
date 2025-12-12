namespace ECommerce.Business.DTOs.Users
{
    public class UserDetailsDto //Used In Admin =>  User Details , Customer => User Account Details
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // map
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int TotalOrders { get; set; } // map 
        public List<string> Roles { get; set; } = []; //map

    }
}
