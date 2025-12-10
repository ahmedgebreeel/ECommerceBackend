using ECommerce.Business.DTOs.Categories;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Categories Management")]
    public class CategoriesController(ICategoryService categories) : ControllerBase
    {
        private readonly ICategoryService _categories = categories;


        [HttpGet]
        [EndpointSummary("Get all categories")]
        [EndpointDescription("Retrieves the full list of product categories available in the store.")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll() => Ok(await _categories.GetAllAsync());

        [HttpGet("{id:int}")]
        [EndpointSummary("Get category details")]
        [EndpointDescription("Retrieves a specific category by its unique ID.")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _categories.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create category")]
        [EndpointDescription("Creates a new product category. Restricted to Administrators.")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var createdCategory = await _categories.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update category")]
        [EndpointDescription("Updates an existing category's details. Restricted to Administrators.")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto)
        {
            var updatedCategory = await _categories.UpdateAsync(id, dto);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete category")]
        [EndpointDescription("Permanently removes a category. Fails with 409 Conflict if products are linked to this category. Restricted to Administrators.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _categories.DeleteAsync(id);
            return NoContent();
        }
    }
}
