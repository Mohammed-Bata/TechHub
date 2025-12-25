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
using TechHub.Domain.Entities;
using TechHub.Infrastructure.Repositories;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressService _addressService;
        private readonly APIResponse _response;
        public AddressesController(AddressService addressService)
        {
            _addressService = addressService;
            _response = new APIResponse();
        }

       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> GetAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var Addresses = await _addressService.GetAddresses(userId);
            _response.Data = Addresses;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

     
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var address = await _addressService.GetAddress(id, userId);
            if (address == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { "Address Not Found" };
                return NotFound(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Data = address;

            return Ok(_response);
        }

       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> CreateAddress([FromBody] AddressDto addressDto,
            IValidator<AddressDto> validator)
        {
            var validationResult = await validator.ValidateAsync(addressDto);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors = new List<string> { "User not authorized" };
                return Unauthorized(_response);
            }

            var entity = await _addressService.CreateAddress(addressDto, userId);

            _response.Data = entity;
            _response.StatusCode = HttpStatusCode.Created;

            return Ok(_response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> UpdateAddress(int id,
            [FromBody] AddressDto addressdto,
            IValidator<AddressDto> validator)
        {
            var validationResult = await validator.ValidateAsync(addressdto);
            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var address = await _addressService.UpdateAddress(id, addressdto);

            _response.Data = addressdto;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

      
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<APIResponse>> DeleteAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors = new List<string> { "User not authorized" };
                return Unauthorized(_response);
            }

            var address = await _addressService.GetAddress(id, userId);
            if (address == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { "Address Not Found" };
                return NotFound(_response);
            }

            var result = await _addressService.DeleteAddress(id, userId);
            if (!result)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Error deleting address" };
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
    }
}
