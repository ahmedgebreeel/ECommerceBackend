using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Orders;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrderService orders) : ControllerBase
    {
        private readonly IOrderService _orders = orders;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _orders.GetAllAsync());


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
            => Ok(await _orders.GetByIdAsync(id));


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var createdOrder = await _orders.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var updatedOrder = await _orders.UpdateStatusAsync(id, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _orders.DeleteAsync(id);
            return NoContent();
        }

    }
}
