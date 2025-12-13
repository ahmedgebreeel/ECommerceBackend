using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            return StatusCode(StatusCodes.Status201Created, userCreated);
        }

        [HttpPost("[action]")]
        [EndpointName("LoginUser")]
        [EndpointSummary("Authenticate user")]
        [EndpointDescription("Validates credentials (username/email + password) and returns a JWT Access Token.")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Input Validation
        [ProducesResponseType(typeof(ApiErrorResponseDto), statusCode: StatusCodes.Status401Unauthorized)] // Wrong Password/User not found
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var authResponse = await _authService.LoginAsync(dto);
            SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);
            return Ok(new LoginResponseDto
            {
                AccessToken = authResponse.AccessToken,
                User = authResponse.User
            });
        }

        [HttpPost("refresh-token")]
        [EndpointSummary("Refresh Access Token")]
        [EndpointDescription("Exchanges a valid Refresh Token for a new pair of Access/Refresh tokens.")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ApiErrorResponseDto
                {
                    StatusCode = 401,
                    Message = "No refresh token provided."
                });

            var authResponse = await _authService.RefreshTokenAsync(refreshToken);

            SetRefreshTokenCookie(authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

            return Ok(new LoginResponseDto
            {
                AccessToken = authResponse.AccessToken,
                User = authResponse.User
            });
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [EndpointSummary("Revoke Token (Logout)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeToken()
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new ApiErrorResponseDto { StatusCode = 400, Message = "Token is required" });

            // 1. Revoke in DB
            await _authService.RevokeTokenAsync(token);

            // 2. Delete the Cookie from the browser
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return NoContent();
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expires,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
