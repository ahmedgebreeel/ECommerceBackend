using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        //one to many relation with Orders
        public virtual ICollection<Order> Orders { get; set; } = [];

    }
}
