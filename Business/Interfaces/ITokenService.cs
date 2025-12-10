using ECommerce.Core.Entities;

namespace ECommerce.Business.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, ICollection<string> roles);
    }
}
