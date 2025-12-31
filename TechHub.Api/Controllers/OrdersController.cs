using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Orders.Commands.CreateOrder;
using TechHub.Application.Orders.Queries.GetOrder;
using TechHub.Application.Orders.Queries.GetOrders;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
       private readonly IMediator _mediator;
       
        public OrdersController(IMediator mediator)
        {
           _mediator = mediator;
        }
        
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(Guid id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var query = new GetOrderQuery(id, userId);

            var order =  await _mediator.Send(query);

            return order;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<OrderResponseDto>>> GetOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var query = new GetOrdersQuery(userId);

            var orders = await _mediator.Send(query);
            return orders;
        }


        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> CreateOrder(OrderDto orderDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var command = new CreateOrderCommand(userId, orderDto.AddressId);

            var orderId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetOrder), new { id = orderId }); 
        }
    }
}
