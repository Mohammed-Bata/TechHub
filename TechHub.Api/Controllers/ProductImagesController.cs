using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechHub.Application.DTOs;
using TechHub.Application.ProductImages.Commands.AddProductImage;
using TechHub.Application.ProductImages.Commands.DeleteProductImage;
using TechHub.Application.ProductImages.Queries.GetProductImage;
using TechHub.Application.ProductImages.Queries.GetProductImages;


namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
       private readonly IMediator _mediator;
        public ProductImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductImageResponseDto>>> GetProductImages(Guid productId)
        {
            var query = new GetProductImagesQuery(productId);

            var images = await _mediator.Send(query);
            return images;
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductImageResponseDto>> GetProductImage(int id)
        {
            var query = new GetProductImageQuery(id);

            var image = await _mediator.Send(query);

            return image;
        }

        
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> PostProductImage(Guid productId, [FromForm] ProductImageDto productImageDto)
        {  
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
           
            var command = new AddProductImageCommand(productId, productImageDto, baseUrl);
            var ImageId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetProductImage),new { id = ImageId },ImageId);
        }

       
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProductImage(int id)
        {        
            var command = new DeleteProductImageCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
