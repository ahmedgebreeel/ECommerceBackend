using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Orders Management")]
    public class OrdersController(IOrderService orders) : ControllerBase
    {
        private readonly IOrderService _orders = orders;

        [HttpGet]
        [EndpointSummary("Get all orders")]
        [EndpointDescription("Retrieves orders. Admins see ALL orders; Customers see only their OWN orders.")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
            => Ok(await _orders.GetAllAsync());


        [HttpGet("{id:int}")]
        [EndpointSummary("Get order details")]
        [EndpointDescription("Retrieves a specific order. Customers can only access their own order ID.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
            => Ok(await _orders.GetByIdAsync(id));


        [HttpPost]
        [EndpointSummary("Place a new order")]
        [EndpointDescription("Creates a new order for the authenticated user. Deducts stock from products automatically.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var createdOrder = await _orders.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update order status")]
        [EndpointDescription("Updates the status of an order (e.g., to Shipped or Delivered). Restricted to Administrators.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)] // Enum Validation
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)] // Non-Admins
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var updatedOrder = await _orders.UpdateStatusAsync(id, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete an order")]
        [EndpointDescription("Permanently deletes an order. Restricted to Administrators.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _orders.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("checkout")]
        [EndpointSummary("Checkout Cart")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            var createdOrder = await _orders.CheckoutAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

    }
}
