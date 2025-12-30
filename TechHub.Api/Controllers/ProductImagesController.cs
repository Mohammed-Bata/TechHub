using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
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
        public async Task<ActionResult<APIResponse>> PostProductImage(Guid productId, [FromForm] ProductImageDto productImageDto)
        {  
            if (productImageDto == null || productImageDto.Image.Length == 0)
            {
                _response.Errors = new List<string> { "No Image Uploaded" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(productImageDto.Image.FileName);
            string imagePath = @"wwwroot/images/" + imageName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            FileInfo file = new FileInfo(directoryLocation);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(directoryLocation, FileMode.Create))
            {
                await productImageDto.Image.CopyToAsync(stream);
            }
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            ProductImage image1 = new()
            {
                ProductId = productId,
                ImageUrl = baseUrl + "/images/" + imageName,
                ImageLocalPath = imagePath
            };

            await _unitOfWork.ProductImages.AddAsync(image1);
            await _unitOfWork.SaveChangesAsync();

            string cacheKey = $"productImages_{productId}";
            await _cache.RemoveAsync(cacheKey);

            _response.Data = image1;
            _response.StatusCode = HttpStatusCode.Created;

            return Ok(_response);  
        }

       
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var image = await _unitOfWork.ProductImages.GetAsync(i => i.ImageId == id);
            if (image == null)
            {
                _response.Errors = new List<string> { "Image not found" };
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), image.ImageLocalPath);
            FileInfo file = new FileInfo(oldFilePathDirectory);

            if (file.Exists)
            {
                file.Delete();
            }

            await _unitOfWork.ProductImages.RemoveAsync(p => p.ImageId == id);
            await _unitOfWork.SaveChangesAsync();

            string cacheKey = $"productImages_{image.ProductId}";
            await _cache.RemoveAsync(cacheKey);

            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);            
        }
    }
}
