using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Services;
using TechHub.Domain;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        private readonly APIResponse _response;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
            _response = new APIResponse();
        }

       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetReviews(int productId, int pageSize = 5, int pageNumber = 1)
        {
            var reviews = await _reviewService.GetReviews(productId, pageSize, pageNumber);

            _response.Data = reviews;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> PostReview(int productId, [FromBody] ReviewDto reviewDto, IValidator<ReviewDto> validator)
        {
            var validationResult = await validator.ValidateAsync(reviewDto);
            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existReview = await _reviewService.PostReview(productId, reviewDto, userId);

            if (existReview == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "User already reviewed this product." };
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.Created;
            _response.Data = existReview;
            return Ok(_response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> PutReview(int id, [FromBody] ReviewDto reviewDto, IValidator<ReviewDto> validator)
        {
            var validationResult = await validator.ValidateAsync(reviewDto);
            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                return BadRequest(_response);
            }
            var review = await _reviewService.GetReview(id);

            if (review is null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { "Review not found." };

                return NotFound(_response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (review.UserId != userId)
            {
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.Errors = new List<string> { "User is not authorized to update this review." };

                return BadRequest(_response);
            }
            review.Content = reviewDto.Content;
            review.Rating = reviewDto.Rating;
            var result = await _reviewService.PutReview(id, reviewDto);

            if (!result)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Failed to update review." };

                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

       
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> DeleteReview(int id)
        {
            var review = await _reviewService.GetReview(id);
            if (review == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { "Review not found." };
                return NotFound(_response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId)
            {
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.Errors = new List<string> { "User is not authorized to delete this review." };
                return BadRequest(_response);
            }

            var result = await _reviewService.DeleteReview(id);
            if (!result)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Failed to delete review." };
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
