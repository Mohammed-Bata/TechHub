using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Users.Commands.Login;
using TechHub.Application.Users.Commands.Register;
using TechHub.Application.Validators;
using TechHub.Domain.Entities;
using TechHub.Infrastructure.Repositories;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        public UsersController(IMediator mediator, ITokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }

       
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterationRequest model)
        {
            var command = new RegisterCommand
            (
              model.FirstName,
              model.LastName,
              model.Email,
              model.UserName,
              model.DateOfBirth,
              model.Password
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
      
        //[HttpPost("Refresh")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] Tokens model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var tokens = await _tokenService.RefreshAccessToken(model);

        //        if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.Errors.Add("Invalid token");

        //            return BadRequest(_response);
        //        }
        //        _response.StatusCode = HttpStatusCode.OK;
        //        _response.Data = tokens;

        //        return Ok(_response);
        //    }
        //    else
        //    {
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.Errors.Add("Invalid token");

        //        return BadRequest(_response);
        //    }
        //}q
        
        //[HttpPost("Logout")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Logout([FromBody] Tokens model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _tokenService.RevokeRefreshToken(model);
        //        _response.StatusCode = HttpStatusCode.OK;
        //        return Ok(_response);
        //    }
        //    _response.StatusCode = HttpStatusCode.BadRequest;
        //    _response.Errors.Add("Invalid Input");
        //    return BadRequest(_response);
        //}
    }
}
