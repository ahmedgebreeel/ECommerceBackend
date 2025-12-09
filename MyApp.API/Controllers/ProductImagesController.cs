using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.ProductImages;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/products/{productId:int}/images")]
    [ApiController]
    public class ProductImagesController(IProductImageService productImages) : ControllerBase
    {
        private readonly IProductImageService _productImages = productImages;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute] int productId)
            => Ok(await _productImages.GetAllAsync(productId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromRoute] int productId, [FromBody] AddProductImageDto dto)
            => CreatedAtAction(nameof(GetAll), new { productId }, await _productImages.AddImageAsync(productId, dto));



        [HttpPut("{imageId:int}/set-main")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetMainImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.SetMainImageAsync(productId, imageId);
            return NoContent();
        }

        [HttpDelete("{imageId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.DeleteImageAsync(productId, imageId);
            return NoContent();
        }



    }
}
