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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _products.GetAllAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var product = await _products.GetByIdAsync(id);
            if (product is null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var createdProduct = await _products.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
        {
            var updatedProduct = await _products.UpdateAsync(id, dto);
            if (updatedProduct is null)
                return NotFound();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var isDeleted = await _products.DeleteAsync(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
    }
}
