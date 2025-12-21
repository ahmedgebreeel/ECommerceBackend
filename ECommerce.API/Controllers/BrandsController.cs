using ECommerce.Business.DTOs.Brands.Admin;
using ECommerce.Business.DTOs.Brands.Store;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Brands;
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

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all brands with paging and search support.")]
        [ProducesResponseType(typeof(PagedResponseDto<AdminBrandDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllBrandsAsmin([FromQuery] AdminBrandSpecParams specParams)
        {
            var brands = await _brands.GetAllBrandsAdminAsync(specParams);
            return Ok(brands);
        }

        [HttpGet("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get brand details.")]
        [ProducesResponseType(typeof(AdminBrandDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBrandDetailsAdmin([FromRoute] int brandId)
        {
            var brand = await _brands.GetBrandDetailsAdminAsync(brandId);
            return Ok(brand);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create a new brand.")]
        [ProducesResponseType(typeof(AdminBrandDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBrandAdmin([FromBody] AdminCreateBrandDto dto)
        {
            var brandCreated = await _brands.CreateBrandAdminAsync(dto);
            return CreatedAtAction(nameof(GetBrandDetailsAdmin), new { brandCreated.Id }, brandCreated);
        }


        [HttpPost("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update existing brand.")]
        [ProducesResponseType(typeof(AdminBrandDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBrandAdmin([FromRoute] int brandId, [FromBody] AdminUpdateBrandDto dto)
        {
            var brandUpdated = await _brands.UpdateBrandAdminAsync(brandId, dto);
            return Ok(brandUpdated);
        }

        [HttpDelete("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete existing brand if no products are referencing it.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteBrandAdmin([FromRoute] int brandId)
        {
            await _brands.DeleteBrandAdminAsync(brandId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all brands.")]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brands.GetAllBrandsAsync();
            return Ok(brands);
        }

    }
}
