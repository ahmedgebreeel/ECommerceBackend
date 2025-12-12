using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Products Management")]
    public class ProductsController(IProductService products) : ControllerBase
    {
        private readonly IProductService _products = products;

        [HttpGet]
        [EndpointSummary("Get all products")]
        [EndpointDescription("Retrieves a list of all products.")]
        [ProducesResponseType(typeof(PagedResponseDto<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ProductSpecParams specParams) => Ok(await _products.GetAllAsync(specParams));

        [HttpGet("{id:int}")]
        [EndpointSummary("Get product details")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _products.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create product")]
        [EndpointDescription("Creates a new product. Validates Brand and Category existence.")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Validation
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)] // Brand/Category not found (Service throws NotFound)
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var createdProduct = await _products.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update product")]
        [EndpointDescription("Updates product details. Concurrency safe.")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
        {
            var updatedProduct = await _products.UpdateAsync(id, dto);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete product")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _products.DeleteAsync(id);
            return NoContent();
        }
    }
}
