using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;
        private readonly ICacheService _cache;

        public ShoppingCartsController(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _response = new APIResponse();
        }

        /// <summary>
        /// Adds an item to the shopping cart for the authenticated user.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>An APIResponse indicating the result of the operation.</returns>
        [HttpPost("AddItem")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> AddItemAsync([FromQuery] int productId,
            [FromQuery] int quantity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _unitOfWork.Carts.AddItemAsync(userId, productId, quantity);

            if (item != null)
            {
                await _unitOfWork.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = item;

                await _cache.RemoveAsync($"Cart_{userId}");
                return Ok(_response);
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.Errors = new List<string> { "Item not added to cart." };

            return BadRequest(_response);
        }

        /// <summary>
        /// Removes an item from the shopping cart for the authenticated user.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        /// <returns>An APIResponse indicating the result of the operation.</returns>
        [HttpDelete("RemoveItem/{itemId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> RemoveItemAsync(int itemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _unitOfWork.Carts.RemoveItemAsync(userId, itemId);

            if (result)
            {
                await _unitOfWork.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = result;

                await _cache.RemoveAsync($"Cart_{userId}");

                return Ok(_response);
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.Errors = new List<string> { "Item not removed from cart." };

            return BadRequest(_response);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetCartWithItemsAsync()
        {
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string cacheKey = $"Cart_{userId}";
            var cachedCart = await _cache.GetAsync<ShoppingCart>(cacheKey);
            if (cachedCart != null)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = cachedCart;
                return Ok(_response);
            }

            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            _response.StatusCode = HttpStatusCode.OK;
            _response.Data = cart;
            await _cache.SetAsync(cacheKey, cart);
            return Ok(_response);
        }
    }
}
