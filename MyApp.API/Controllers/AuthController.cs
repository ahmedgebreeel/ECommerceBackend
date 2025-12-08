using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Auth;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userCreated = await _authService.RegisterAsync(dto);
            return Ok(userCreated);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var isLogged = await _authService.LoginAsync(dto);
            return Ok(isLogged);
        }
    }
}
