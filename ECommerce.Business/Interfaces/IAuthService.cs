using ECommerce.Business.DTOs.Auth.Requests;
using ECommerce.Business.DTOs.Auth.Responses;

namespace ECommerce.Business.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResponse, string, DateTime)> RegisterAsync(RegisterRequest registerRequest);
        Task<(AuthResponse, string, DateTime)> LoginAsync(LoginRequest loginRequest);
        Task<(AuthResponse, string, DateTime)> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string token);
    }
}
