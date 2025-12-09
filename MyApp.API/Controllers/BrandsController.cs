using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Brands;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController(IBrandService brands) : ControllerBase
    {
        private readonly IBrandService _brands = brands;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _brands.GetAllAsync());


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _brands.GetByIdAsync(id));


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            var createdBrand = await _brands.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBrandDto dto)
        {
            var updatedBrand = await _brands.UpdateAsync(id, dto);
            return Ok(updatedBrand);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _brands.DeleteAsync(id);
            return NoContent();
        }

    }
}
