using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Validators;
using TechHub.Domain;
using TechHub.Infrastructure.Repositories;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly APIResponse _response;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        public UsersController(IUserRepository userRepository, IAuthService authService, ITokenService tokenService)
        {
            _userRepo = userRepository;
            _response = new APIResponse();
            _authService = authService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="model">The registration request containing user details.</param>
        /// <param name="validator">The validator for the registration request.</param>
        /// <returns>An ActionResult indicating the result of the registration process.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] RegisterationRequest model,
            IValidator<RegisterationRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            bool ifUserNameUnique = await _userRepo.IsUniqueAsync(model.Email, model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _authService.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        /// <summary>
        /// Authenticates a user and generates access and refresh tokens.
        /// </summary>
        /// <param name="model">The login request containing user credentials.</param>
        /// <param name="validator">The validator for the login request.</param>
        /// <returns>An ActionResult containing the authentication tokens or an error response.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest model,
            IValidator<LoginRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                return BadRequest(_response);
            }

            var tokens = await _authService.Login(model);

            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Username or password is incorrect");

                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Data = tokens;

            return Ok(_response);
        }
        /// <summary>
        /// Refreshes the access token using the provided refresh token.
        /// </summary>
        /// <param name="model">The tokens object containing the access and refresh tokens.</param>
        /// <returns>An ActionResult containing the new tokens or an error response.</returns>
        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] Tokens model)
        {
            if (ModelState.IsValid)
            {
                var tokens = await _tokenService.RefreshAccessToken(model);

                if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add("Invalid token");

                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = tokens;

                return Ok(_response);
            }
            else
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Invalid token");

                return BadRequest(_response);
            }
        }
        /// <summary>
        /// Logs out a user by revoking the provided refresh token.
        /// </summary>
        /// <param name="model">The tokens object containing the access and refresh tokens.</param>
        /// <returns>An ActionResult indicating the result of the logout process.</returns>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] Tokens model)
        {
            if (ModelState.IsValid)
            {
                await _tokenService.RevokeRefreshToken(model);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.Errors.Add("Invalid Input");
            return BadRequest(_response);
        }
    }
}
