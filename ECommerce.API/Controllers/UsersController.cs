using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Users.Requests;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Users Management")]
    [Authorize]
    public class UsersController(IUserService users) : ControllerBase
    {
        private readonly IUserService _users = users;


        [HttpGet]
        [EndpointSummary("Retrieves logged in user details.")]
        [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails()
        {
            var userDetails = await _users.GetDetailsAsync();
            return Ok(userDetails);
        }

        [HttpPut("image")]
        [EndpointSummary("Updates logged in user profile image.")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateImage([FromForm] UploadImageRequest uploadImageRequest)
        {
            var userDetails = await _users.UpdateImageAsync(uploadImageRequest);
            return Ok(userDetails);
        }

        [HttpDelete("image")]
        [EndpointSummary("User removes his profile image.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage()
        {
            await _users.DeleteImageAsync();
            return NoContent();
        }

        [HttpPut]
        [EndpointSummary("User updates his account info.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePersonalInfo([FromBody] UpdateUserRequest updateUserRequest)
        {
            var authResponse = await _users.UpdatePersonalInfoAsync(updateUserRequest);
            return Ok(authResponse);
        }

        [HttpPut("password")]
        [EndpointSummary("User updates his password.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            var (authResponse, refreshToken, refreshTokenExpiration) = await _users.UpdatePasswordAsync(updatePasswordRequest);
            SetRefreshTokenCookie(refreshToken, refreshTokenExpiration);
            return Ok(authResponse);

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
