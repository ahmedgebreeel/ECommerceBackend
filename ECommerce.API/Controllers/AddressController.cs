using ECommerce.Business.DTOs.Addresses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Addresses Managment")]
    public class AddressController(IAddressService addresses) : ControllerBase
    {
        private readonly IAddressService _addresses = addresses;

        [HttpGet]
        [EndpointSummary("Get addresses")]
        [EndpointDescription(
        "Retrieves a list of addresses. Behavior depends on the user role:\n" +
        "- **Customers**: Returns only their own saved addresses.\n" +
        "- **Admins (No Filter)**: Returns ALL system addresses with user details (`AddressWithUserDto`).\n" +
        "- **Admins (with `userId`)**: Returns addresses for that specific user only.")]
        [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] string? userId)
            => Ok(await _addresses.GetAllAsync(userId));

        [HttpGet("{id:int}")]
        [EndpointSummary("Get address details")]
        [EndpointDescription("Retrieves a specific address by ID. Validates that the user owns the address (or is Admin).")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
            => Ok(await _addresses.GetByIdAsync(id));

        [HttpPost]
        [EndpointSummary("Create new address")]
        [EndpointDescription("Adds a new address to the authenticated user's profile.")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            var createdAddress = await _addresses.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdAddress.Id }, createdAddress);
        }

        [HttpPut("{id:int}")]
        [EndpointSummary("Update address")]
        [EndpointDescription("Updates an existing address. Users can only update addresses they created.")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateAddressDto dto)
        {
            var updatedAddress = await _addresses.UpdateAsync(id, dto);
            return Ok(updatedAddress);
        }

        [HttpDelete("{id:int}")]
        [EndpointSummary("Delete address")]
        [EndpointDescription("Permanently removes an address. Users can only delete their own addresses.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _addresses.DeleteAsync(id);
            return NoContent();
        }
    }
}
