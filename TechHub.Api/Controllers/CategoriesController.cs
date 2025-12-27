using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechHub.Application.Categories.Commands;
using TechHub.Application.Categories.Commands.CreateCategory;
using TechHub.Application.Categories.Commands.DeleteCategory;
using TechHub.Application.Categories.Queries.GetCategories;
using TechHub.Application.Categories.Queries.GetCategory;
using TechHub.Application.DTOs;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
      

        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var query = new GetCategoryQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = new GetCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }


            [HttpPost]
            [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CategoryDto category)
        {
            var command = new CreateCategoryCommand(category.Name);
            var Id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = Id },category);

        }


        //    [HttpPut("{id}")]
        //    [Authorize(Roles = "Admin")]
        //    [ProducesResponseType(StatusCodes.Status200OK)]
        //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //    [ProducesResponseType(StatusCodes.Status404NotFound)]
        //    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //    public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] CategoryDto categoryDto,
        //        IValidator<CategoryDto> validator)
        //    {
        //        var validationResult = await validator.ValidateAsync(categoryDto);

        //        if (!validationResult.IsValid)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        //            return BadRequest(_response);
        //        }

        //        if (await _unitOfWork.Categories.GetAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower()) != null)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.Errors.Add("Category already exists");
        //            return BadRequest(_response);
        //        }

        //        var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
        //        if (category == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            _response.Errors.Add("Category Not Found");
        //            return NotFound();
        //        }
        //        category.Name = categoryDto.Name;
        //        await _unitOfWork.SaveChangesAsync();
        //        _response.Data = categoryDto;
        //        _response.StatusCode = HttpStatusCode.OK;

        //        await _cache.RemoveAsync("categories");

        //        return Ok(_response);

        //    }


            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteCategoryCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
