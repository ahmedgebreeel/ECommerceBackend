using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.Orders.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Orders;
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


        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Seller")]
        [EndpointSummary("Get all orders for admin dashboard.")]
        [ProducesResponseType(typeof(PagedResponseDto<AdminOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrdersAdmin([FromQuery] AdminOrderSpecParams specParams)
            => Ok(await _orders.GetAllOrdersAdminAsync(specParams));



        [HttpGet("admin/{orderId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        [EndpointSummary("Get order details for an order.")]
        [ProducesResponseType(typeof(AdminOrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderDetailsAdmin([FromRoute] int orderId)
            => Ok(await _orders.GetOrderDetailsAdminAsync(orderId));


        [HttpPut("admin/{orderId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        [EndpointSummary("Update order status , shipping address (if applicable).")]
        [ProducesResponseType(typeof(AdminOrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderAdmin([FromRoute] int orderId, [FromBody] AdminUpdateOrderDto dto)
        {
            var updatedOrder = await _orders.UpdateOrderAdminAsync(orderId, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("admin/{orderId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        [EndpointSummary("Delete an order.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderAdmin([FromRoute] int orderId)
        {
            await _orders.DeleteOrderAdminAsync(orderId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all orders of logged in user.")]
        [ProducesResponseType(typeof(PagedResponseDto<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderSpecParams specParams)
            => Ok(await _orders.GetAllOrdersAsync(specParams));

        [HttpPost("checkout")]
        [EndpointSummary("Checkout Cart")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            var createdOrder = await _orders.CheckoutAsync(dto);
            return StatusCode(StatusCodes.Status201Created, createdOrder);
        }
    }
}
