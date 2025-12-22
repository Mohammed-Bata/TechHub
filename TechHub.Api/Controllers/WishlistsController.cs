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
    public class WishlistsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;
        private readonly ICacheService _cache;

        public WishlistsController(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _response = new APIResponse();
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetWishlist()
        {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "User ID not found" };
                    return BadRequest(_response);
                }
            string cacheKey = $"Wishlist_{userId}";
            var cachedWishlist = await _cache.GetAsync<Wishlist>(cacheKey);
            if (cachedWishlist != null)
            {
                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = cachedWishlist;
                return Ok(_response);
            }
         
           var wishlist = await _unitOfWork.Wishlists.GetWishlistByUserId(userId);
               
                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = wishlist;
               
                await _cache.SetAsync(cacheKey, wishlist);
                return Ok(_response);
        }

       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> AddToWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "User ID not found" };

                return BadRequest(_response);
            }

            bool Added = await _unitOfWork.Wishlists.AddProductToWishlist(productId, userId);

            await _unitOfWork.SaveChangesAsync();

            _response.Data = Added;
            _response.StatusCode = HttpStatusCode.OK;

            string cacheKey = $"Wishlist_{userId}";

           await _cache.RemoveAsync(cacheKey);

                return Ok(_response);
        }

       
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RemoveWishlistProduct(int productId)
        {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "User ID not found" };
                    return BadRequest(_response);
                }

                bool Removed = await _unitOfWork.Wishlists.RemoveFromWishlist(productId, userId);
                await _unitOfWork.SaveChangesAsync();

                _response.Data = Removed;
                _response.StatusCode = HttpStatusCode.OK;
            string cacheKey = $"Wishlist_{userId}";

            await _cache.RemoveAsync(cacheKey);

            return Ok(_response);
           
        }
    }
}
