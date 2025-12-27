using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using TechHub.Application.Addresses.Commands.CreateAddress;
using TechHub.Application.Addresses.Commands.DeleteAddress;
using TechHub.Application.Addresses.Commands.UpdateAddress;
using TechHub.Application.Addresses.Queries.GetAddress;
using TechHub.Application.Addresses.Queries.GetAddresses;
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
       

        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[Authorize]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var addresses = await _mediator.Send(new GetAddressesQuery(userId));
            return Ok(addresses);

        }

     
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> GetAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var address = await _mediator.Send(new GetAddressQuery(id, userId));

            return Ok(address);
        }

       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> CreateAddress([FromBody] AddressDto addressDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var addressId = await _mediator.Send(new CreateAddressCommand(
                addressDto.Street,
                addressDto.City,
                addressDto.Governorate,
                addressDto.PostalCode,
                userId));

            return CreatedAtAction(nameof(GetAddress), new { id = addressId }, addressDto);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(int id,
            [FromBody] AddressDto addressdto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var addressId = await _mediator.Send(new UpdateAddressCommand(
                id,
                addressdto.Street,
                addressdto.City,
                addressdto.Governorate,
                addressdto.PostalCode,
                userId));

            return Ok(addressId);


        }

      
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var addressId = await _mediator.Send(new DeleteAddressCommand(id));

            return NoContent();
        }
    }
}
