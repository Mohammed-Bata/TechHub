using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.ProductImages.Commands.AddProductImage;
using TechHub.Application.ProductImages.Commands.DeleteProductImage;
using TechHub.Application.ProductImages.Queries.GetProductImage;
using TechHub.Application.ProductImages.Queries.GetProductImages;
using TechHub.Domain.Entities;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<List<ProductImageResponseDto>>> GetProductImages(Guid productId)
        {
            var query = new GetProductImagesQuery(productId);

            var images = await _mediator.Send(query);
            return images;
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<ProductImageResponseDto>> GetProductImage(int id)
        {
            var query = new GetProductImageQuery(id);

            var image = await _mediator.Send(query);

            return image;
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<int>> PostProductImage(Guid productId, [FromForm] ProductImageDto productImageDto)
        {  
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
           
            var command = new AddProductImageCommand(productId, productImageDto, baseUrl);
            var ImageId = await _mediator.Send(command);

            return ImageId; 
        }

       
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult> DeleteProductImage(int id)
        {        
            var command = new DeleteProductImageCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
