

using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        //Parent(ApplicationUser) -> Child

        //One to One Relation with Wishlist ( ApplicationUser (1) -> (1) Wishlist )
        public virtual Wishlist Wishlist { get; set; } = null!;

        //One to One Relation with ShoppingCart ( ApplicationUser (1) -> (1) ShoppingCart )
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        //One to Many Relation with Order ( ApplicationUser (1) -> (N) Order )
        public virtual ICollection<Order> Orders { get; set; } = [];

        //One to Many Relation with Address ( ApplicationUser (1) -> (N) Address )
        public virtual ICollection<Address> Addresses { get; set; } = [];

        //One to Many Relation with RefreshToken ( ApplicationUser (1) -> (N) RefreshToken)
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    }
}
