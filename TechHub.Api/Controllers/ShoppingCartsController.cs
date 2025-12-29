using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TechHub.Application.Carts.Commands.AddToCart;
using TechHub.Application.Carts.Commands.RemoveFromCart;
using TechHub.Application.Carts.Queries.GetCart;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
       private readonly IMediator _mediator;

        public ShoppingCartsController(IMediator mediator)
        {
                _mediator = mediator;
        }


        [HttpPost("AddItem")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddItemAsync([FromQuery] Guid productId,
            [FromQuery] int quantity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            var command = new AddToCartCommand(userId, productId, quantity);

            var cartItemId = await _mediator.Send(command);
            return Ok(cartItemId);
        }


        [HttpDelete("RemoveItem/{itemId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Guid>> RemoveItemAsync(Guid itemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var command = new RemoveFromCartCommand(userId, itemId);

            await _mediator.Send(command);

            return NoContent();


        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShoppingCartDto>> GetCartWithItemsAsync()
        {
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var query = new GetCartQuery(userId);

            var cart = await _mediator.Send(query);

            return cart;
        }
    }
}
