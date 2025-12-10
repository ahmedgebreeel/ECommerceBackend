using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/products/{productId:int}/images")]
    [ApiController]
    [Tags("Product Images")]
    public class ProductImagesController(IProductImageService productImages) : ControllerBase
    {
        private readonly IProductImageService _productImages = productImages;

        [HttpGet]
        [EndpointSummary("Get product images")]
        [ProducesResponseType(typeof(IEnumerable<ProductImageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromRoute] int productId)
            => Ok(await _productImages.GetAllAsync(productId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Add image")]
        [EndpointDescription("Adds an image URL to a product.")]
        [ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add([FromRoute] int productId, [FromBody] AddProductImageDto dto)
            => CreatedAtAction(nameof(GetAll), new { productId }, await _productImages.AddImageAsync(productId, dto));



        [HttpPut("{imageId:int}/set-main")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Set main image")]
        [EndpointDescription("Updates the product's main thumbnail image.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SetMainImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.SetMainImageAsync(productId, imageId);
            return NoContent();
        }

        [HttpDelete("{imageId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete image")]
        [EndpointDescription("Removes an image. Cannot delete the Main image.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.DeleteImageAsync(productId, imageId);
            return NoContent();
        }



    }
}
