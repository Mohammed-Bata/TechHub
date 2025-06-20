using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;
        private readonly ICacheService _cache;

        public ProductsController(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _response = new APIResponse();
        }

        /// <summary>
        /// Searches for products based on the provided query parameters.
        /// </summary>
        /// <param name="queryParameters">The parameters to filter the products.</param>
        /// <param name="pageSize">The number of products per page. Default is 9.</param>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <returns>An APIResponse containing the search results.</returns>
        [HttpGet("search")]        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> SearchProducts([FromQuery] ProductQueryParameters queryParameters, int pageSize = 9, int pageNumber = 1)
        {
            
            var products = await _unitOfWork.Products.GetProducts(includes: o => o.Include("Category").Include(p => p.Reviews), queryParameters.Name, queryParameters.CategoryId, queryParameters.Description, queryParameters.Brand, queryParameters.MinPrice, queryParameters.MaxPrice, queryParameters.ReviewCount, queryParameters.MinAverageRating, pageSize, pageNumber: pageNumber);
            _response.Data = products;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>An APIResponse containing the product details if found, or a 404 status if not found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetProduct(int id)
        {
           
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id, includes: o => o.Include("Category").Include("Reviews"));

            if (product == null)
            {
                _response.Errors = new List<string> { "Product not found" };
                _response.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_response);
            }
            _response.Data = product;
            _response.StatusCode = HttpStatusCode.OK;


            return Ok(_response);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">The product data transfer object containing product details.</param>
        /// <param name="validator">The validator for validating the product data.</param>
        /// <returns>An APIResponse containing the created product details or validation errors.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> CreateProduct(ProductDto productDto,
            IValidator<ProductDto> validator)
        {
            var validationResult = await validator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            if (await _unitOfWork.Products.GetAsync(u => u.Name.ToLower() == productDto.Name.ToLower()) != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Product already exists" };

                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockAmount = productDto.StockAmount,
                Brand = productDto.Brand,
                CategoryId = productDto.CategoryId,
                ProductCode = productDto.ProductCode
            };

            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.CoverImage.FileName);
            string imagePath = @"wwwroot/images/" + imageName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            FileInfo file = new FileInfo(directoryLocation);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(directoryLocation, FileMode.Create))
            {
                await productDto.CoverImage.CopyToAsync(stream);
            }
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImageUrl = baseUrl + "/images/" + imageName;
            product.ImageLocalPath = imagePath;
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _response.Data = product;
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _response);
        }

        /// <summary>
        /// Updates an existing product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="updatedProduct">The updated product data transfer object containing new product details.</param>
        /// <param name="validator">The validator for validating the updated product data.</param>
        /// <returns>An APIResponse containing the updated product details or validation errors.</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id, [FromForm] ProductDto updatedProduct,
            IValidator<ProductDto> validator)
        {
            var validationResult = await validator.ValidateAsync(updatedProduct);
            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                return BadRequest(_response);
            }
            var existProduct = await _unitOfWork.Products.GetAsync(p => p.Id == id);
            if (existProduct == null)
            {
                _response.Errors = new List<string> { "Product not found" };
                _response.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_response);
            }
            existProduct.Name = updatedProduct.Name;
            existProduct.Description = updatedProduct.Description;
            existProduct.Price = updatedProduct.Price;
            existProduct.StockAmount = updatedProduct.StockAmount;
            existProduct.Brand = updatedProduct.Brand;
            existProduct.CategoryId = updatedProduct.CategoryId;
            existProduct.ProductCode = updatedProduct.ProductCode;

            if (updatedProduct.CoverImage != null)
            {
                if (!string.IsNullOrEmpty(existProduct.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), existProduct.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(updatedProduct.CoverImage.FileName);
                string imagepath = @"wwwroot\images\" + filename;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagepath);

                using (var filestream = new FileStream(directoryLocation, FileMode.Create))
                {
                    await updatedProduct.CoverImage.CopyToAsync(filestream);
                }
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                existProduct.ImageUrl = baseUrl + "/images/" + filename;
                existProduct.ImageLocalPath = imagepath;
            }
            await _unitOfWork.SaveChangesAsync();
          

            _response.Data = existProduct;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <returns>An APIResponse indicating the result of the delete operation.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id);
            if (product == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { "Product not found." };
                return NotFound(_response);
            }

            if (!string.IsNullOrEmpty(product.ImageLocalPath))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                FileInfo file = new FileInfo(oldFilePathDirectory);

                if (file.Exists)
                {
                    file.Delete();
                }
            }

            await _unitOfWork.Products.RemoveAsync(p => p.Id == id);
            await _unitOfWork.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
