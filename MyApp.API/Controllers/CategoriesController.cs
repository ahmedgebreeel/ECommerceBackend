using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Categories;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categories) : ControllerBase
    {
        private readonly ICategoryService _categories = categories;


        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _categories.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _categories.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var createdCategory = await _categories.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto)
        {
            var updatedCategory = await _categories.UpdateAsync(id, dto);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _categories.DeleteAsync(id);
            return NoContent();
        }
    }
}
