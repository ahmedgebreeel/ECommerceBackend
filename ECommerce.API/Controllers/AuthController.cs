using ECommerce.Business.DTOs.Auth.Requests;
using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Authentication")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("[action]")]
        [EndpointName("RegisterUser")]
        [EndpointSummary("Register a new user")]
        [EndpointDescription("Creates a new user account with the default 'Customer' role. Requires a unique email and username, login automatically.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequestDto)
        {
            var (authResponse, refreshToken, refreshTokenExpiration) = await _authService.RegisterAsync(registerRequestDto);
            SetRefreshTokenCookie(refreshToken, refreshTokenExpiration);
            return StatusCode(StatusCodes.Status201Created, authResponse);
        }

        [HttpPost("[action]")]
        [EndpointName("LoginUser")]
        [EndpointSummary("Authenticate user")]
        [EndpointDescription("Validates credentials (username/email + password) and returns a JWT Access Token.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var (authResponse, refreshToken, refreshTokenExpiration) = await _authService.LoginAsync(dto);
            SetRefreshTokenCookie(refreshToken, refreshTokenExpiration);
            return Ok(authResponse);
        }

        [HttpPost("refresh-token")]
        [EndpointSummary("Refresh Access Token")]
        [EndpointDescription("Exchanges a valid Refresh Token for a new pair of Access/Refresh tokens.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshToken()
        {
            var existigRefreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(existigRefreshToken))
            {
                _logger.LogWarning("Refresh Token Attempt Failed: No token provided in cookies.");

                return Unauthorized(new ApiErrorResponse
                {
                    StatusCode = 401,
                    Message = "No refresh token provided."
                });
            }
            var (authResponse, refreshToken, refreshTokenExpiration) = await _authService.RefreshTokenAsync(existigRefreshToken);

            SetRefreshTokenCookie(refreshToken, refreshTokenExpiration);

            return Ok(authResponse);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [EndpointSummary("Revoke Token (Logout)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeToken()
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new ApiErrorResponse { StatusCode = 400, Message = "Token is required" });

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
