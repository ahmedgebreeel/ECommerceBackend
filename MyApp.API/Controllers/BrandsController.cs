using ECommerce.Business.DTOs.Brands;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Brands Management")]
    public class BrandsController(IBrandService brands) : ControllerBase
    {
        private readonly IBrandService _brands = brands;

        [HttpGet]
        [EndpointSummary("Get all brands")]
        [EndpointDescription("Retrieves a complete list of all product brands available in the system.")]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll() => Ok(await _brands.GetAllAsync());


        [HttpGet("{id:int}")]
        [EndpointSummary("Get brand details")]
        [EndpointDescription("Retrieves the details of a specific brand by its unique ID.")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _brands.GetByIdAsync(id));


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create a new brand")]
        [EndpointDescription("Creates a new brand. Restricted to Administrators.")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Validation errors
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)] // Not logged in
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)] // Logged in but not Admin
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            var createdBrand = await _brands.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update a brand")]
        [EndpointDescription("Updates an existing brand's name or description. Restricted to Administrators.")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Validation errors
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBrandDto dto)
        {
            var updatedBrand = await _brands.UpdateAsync(id, dto);
            return Ok(updatedBrand);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete a brand")]
        [EndpointDescription("Permanently deletes a brand. Fails if the brand has associated products (returns 409 Conflict). Restricted to Administrators.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Success (No Body)
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)] // Cannot delete because products exist

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _brands.DeleteAsync(id);
            return NoContent();
        }

    }
}
