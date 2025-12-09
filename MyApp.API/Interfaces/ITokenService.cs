using MyApp.API.Entities;

namespace MyApp.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, ICollection<string> roles);
    }
}
