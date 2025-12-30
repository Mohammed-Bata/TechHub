using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Products.Commands.CreateProduct;
using TechHub.Application.Products.Commands.DeleteProduct;
using TechHub.Application.Products.Commands.UpdateProduct;
using TechHub.Application.Products.Queries.GetProduct;
using TechHub.Application.Products.Queries.SearchProducts;
using TechHub.Domain.Entities;
namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
       private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("search")]        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductResponseDto>>> SearchProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            var query = new SearchProductsQuery(queryParameters);

            var products = await _mediator.Send(query);

            return products;
        }

      
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(Guid id)
        {
            var query = new GetProductQuery(id);

            var product = await _mediator.Send(query);

            return product;
        }

        
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> CreateProduct(ProductDto productDto)
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

            
            var command = new CreateProductCommand(productDto, baseUrl);

            var productId = await _mediator.Send(command);
            return productId;
        }

       
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Guid>> UpdateProduct(Guid id, [FromForm] ProductDto updatedProduct)
        {
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

            var command = new UpdateProductCommand(id,updatedProduct, baseUrl);

            var productId = await _mediator.Send(command);

            return productId;
        }



        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var command = new DeleteProductCommand(id);

            await _mediator.Send(command);

            return NoContent();
            
        }
    }
}
