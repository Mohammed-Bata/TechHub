using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Reviews.Commands.CreateReview;
using TechHub.Application.Reviews.Commands.DeleteReview;
using TechHub.Application.Reviews.Commands.UpdateReview;
using TechHub.Application.Reviews.Queries.GetReview;
using TechHub.Application.Reviews.Queries.GetReviews;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
       private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews(Guid productId)
        {
           var query = new GetReviewsQuery(productId);

             var reviews = await _mediator.Send(query);

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewResponseDto>> GetReview(Guid id)
        {
            var query = new GetReviewQuery(id);
            var review = await _mediator.Send(query);
           
            return review;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> PostReview(Guid productId, [FromBody] ReviewDto reviewDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new CreateReviewCommand
            (
               productId,
               userId,
               reviewDto.Content,
                reviewDto.Rating
            );
            var reviewId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetReview), new { id = reviewId });

        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Guid>> PutReview(Guid id, [FromBody] ReviewResponseDto reviewDto)
        {
            var command = new UpdateReviewCommand
            (
               id,
               reviewDto.Content,
               reviewDto.Rating
            );

            var reviewId = await _mediator.Send(command);

            return Ok(reviewId);

        }

       
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
           var command = new DeleteReviewCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
