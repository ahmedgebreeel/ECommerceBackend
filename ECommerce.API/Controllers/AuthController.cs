using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Authentication")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("[action]")]
        [EndpointName("RegisterUser")]
        [EndpointSummary("Register a new user")]
        [EndpointDescription("Creates a new user account with the default 'Customer' role. Requires a unique email and username.")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Input Validation (Missing fields)
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)] // Logic Error (User already exists - if you add this logic later)
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userCreated = await _authService.RegisterAsync(dto);
            return Ok(userCreated);
        }

        [HttpPost("[action]")]
        [EndpointName("LoginUser")]
        [EndpointSummary("Authenticate user")]
        [EndpointDescription("Validates credentials (username/email + password) and returns a JWT Access Token.")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Input Validation
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)] // Wrong Password/User not found
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(token);
        }
    }
}
