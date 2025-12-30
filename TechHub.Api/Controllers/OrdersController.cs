using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;
using System.Net;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Orders.Commands.CreateOrder;
using TechHub.Application.Orders.Queries.GetOrder;
using TechHub.Application.Orders.Queries.GetOrders;
using TechHub.Application.Services;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;
using Order = TechHub.Domain.Entities.Order;
using OrderService = TechHub.Application.Services.OrderService;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
       private readonly IMediator _mediator;
        private readonly IPaymentService _paymentService;
        

        public OrdersController(IPaymentService paymentService,IMediator mediator)
        {
           _mediator = mediator;
            _paymentService = paymentService;
        }

        
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
           
            var command = new CreateOrderCommand(userId,orderDto.AddressId);

            var orderId = await _mediator.Send(command);

            return orderId;
            
        }
    }
}
