using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechHub.Application.Addresses.Commands.CreateAddress;
using TechHub.Application.Addresses.Commands.DeleteAddress;
using TechHub.Application.Addresses.Commands.UpdateAddress;
using TechHub.Application.Addresses.Queries.GetAddress;
using TechHub.Application.Addresses.Queries.GetAddresses;
using TechHub.Application.DTOs;


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
        [Authorize]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var addresses = await _mediator.Send(new GetAddressesQuery(userId));
            return addresses;

        }

     
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var address = await _mediator.Send(new GetAddressQuery(id, userId));

            return address;
        }

       
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<int>> CreateAddress([FromBody] AddressDto addressDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var addressId = await _mediator.Send(new CreateAddressCommand(
                addressDto.Street,
                addressDto.City,
                addressDto.Governorate,
                addressDto.PostalCode,
                userId));

            return addressId;

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<int>> UpdateAddress(int id,
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

            return addressId;


        }

      
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var addressId = await _mediator.Send(new DeleteAddressCommand(id));

            return NoContent();
        }
    }
}
