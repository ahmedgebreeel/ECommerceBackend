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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _brands.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var brand = await _brands.GetByIdAsync(id);
            if (brand is null)
                return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto dto)
        {
            var createdBrand = await _brands.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBrand([FromRoute] int id, [FromBody] UpdateBrandDto dto)
        {
            var updatedBrand = await _brands.UpdateAsync(id, dto);

            if (updatedBrand is null)
                return NotFound();
            return Ok(updatedBrand);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBrand([FromRoute] int id)
        {
            var isDeleted = await _brands.DeleteAsync(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }

    }
}
