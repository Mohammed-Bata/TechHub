﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;
        private readonly ICacheService _cache;

        public ProductImagesController(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _response = new APIResponse();
        }

        /// <summary>
        /// Retrieves all product images for a given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product whose images are to be retrieved.</param>
        /// <returns>An APIResponse containing the list of product images or an error message.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetProductImages(int productId)
        {
            string cacheKey = $"productImages_{productId}";
            var cachedImages = await _cache.GetAsync<List<ProductImage>>(cacheKey);
            if (cachedImages != null)
            {
                _response.Data = cachedImages;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }

            var images = await _unitOfWork.ProductImages.GetAll(i => i.ProductId == productId);

            if (images == null || !images.Any())
            {
                _response.Errors = new List<string> { "No images found for this product." };
                _response.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_response);
            }
            _response.Data = images;
            _response.StatusCode = HttpStatusCode.OK;

            await _cache.SetAsync(cacheKey, images);

            return Ok(_response);
        }

        /// <summary>
        /// Retrieves a specific product image by its ID.
        /// </summary>
        /// <param name="id">The ID of the product image to retrieve.</param>
        /// <returns>An APIResponse containing the product image or an error message.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetProductImage(int id)
        {
            var image = await _unitOfWork.ProductImages.GetAsync(i => i.ImageId == id);

            if (image == null)
            {
                _response.Errors = new List<string> { "Image not found" };
                _response.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_response);
            }
            _response.Data = image;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        /// <summary>
        /// Adds a new product image for a given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to which the image belongs.</param>
        /// <param name="productImageDto">The DTO containing the image file and related data.</param>
        /// <returns>An APIResponse containing the created product image or an error message.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> PostProductImage(int productId, [FromForm] ProductImageDto productImageDto)
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

        /// <summary>
        /// Deletes a specific product image by its ID.
        /// </summary>
        /// <param name="id">The ID of the product image to delete.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
