using MyApp.API.DTOs.Auth;

namespace MyApp.API.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<bool> LoginAsync(LoginDto dto);
    }
}
