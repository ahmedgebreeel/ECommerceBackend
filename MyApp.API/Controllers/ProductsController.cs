using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Products;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService products) : ControllerBase
    {
        private readonly IProductService _products = products;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _products.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) => Ok(await _products.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var createdProduct = await _products.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
        {
            var updatedProduct = await _products.UpdateAsync(id, dto);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _products.DeleteAsync(id);
            return NoContent();
        }
    }
}
