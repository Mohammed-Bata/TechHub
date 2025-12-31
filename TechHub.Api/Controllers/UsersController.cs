using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechHub.Application.DTOs;
using TechHub.Application.Users.Commands.Login;
using TechHub.Application.Users.Commands.Logout;
using TechHub.Application.Users.Commands.Refresh;
using TechHub.Application.Users.Commands.Register;
using TechHub.Domain.Entities;


namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterationRequest model)
        {
            var command = new RegisterCommand
            (
              model
            );

            var user = await _mediator.Send(command);

            return user;
        }

       
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Tokens>> Login([FromBody] LoginRequest model)
        {
            var command = new LoginCommand
            (
              model.Email,
              model.Password
            );

            var tokens = await _mediator.Send(command);

            return tokens;
        }

        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Tokens>> GetNewTokenFromRefreshToken([FromBody] Tokens model)
        {
            var command = new RefreshTokensCommand(model);
            var tokens = await _mediator.Send(command);
            return tokens;
        }

        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Logout([FromBody] Tokens model)
        {
            var command = new LogoutCommand(model);

            var result = await _mediator.Send(command);

            return result;
        }
    }
}
