using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIResponse _response;
        private readonly ICacheService _cache;

        public CategoriesController(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
            _cache = cache;
        }

       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Index()
        {
            string cacheKey = "categories";
            var cachedCategories = await _cache.GetAsync<List<Category>>(cacheKey);
            if (cachedCategories != null)
            {
                _response.Data = cachedCategories;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            var categories = await _unitOfWork.Categories.GetAll();
            _response.Data = categories;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] CategoryDto category,
            IValidator<CategoryDto> validator)
        {
            var validationResult = await validator.ValidateAsync(category);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var entity = await _unitOfWork.Categories.GetAsync(c => c.Name.ToLower() == category.Name.ToLower());
            if (entity != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Category already exists");
                return BadRequest(_response);
            }

            var entity2 = new Category
            {
                Name = category.Name
            };
            await _unitOfWork.Categories.AddAsync(entity2);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("categories");

            _response.Data = entity2;
            _response.StatusCode = HttpStatusCode.Created;

            return Ok(_response);
        }

       
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] CategoryDto categoryDto,
            IValidator<CategoryDto> validator)
        {
            var validationResult = await validator.ValidateAsync(categoryDto);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            if (await _unitOfWork.Categories.GetAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower()) != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Category already exists");
                return BadRequest(_response);
            }

            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors.Add("Category Not Found");
                return NotFound();
            }
            category.Name = categoryDto.Name;
            await _unitOfWork.SaveChangesAsync();
            _response.Data = categoryDto;
            _response.StatusCode = HttpStatusCode.OK;

            await _cache.RemoveAsync("categories");

            return Ok(_response);

        }

       
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors.Add("Category Not Found");
                return NotFound();
            }

            await _unitOfWork.Categories.RemoveAsync(c => c.Id == id);
            await _unitOfWork.SaveChangesAsync();
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}
