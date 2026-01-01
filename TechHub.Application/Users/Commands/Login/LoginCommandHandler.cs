using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Users.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Tokens>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }
        public async Task<Tokens> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
           
          
            var user = await _authService.CheckPasswordAsync(command.Email, command.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var jwtTokenId = Guid.NewGuid().ToString();
            var accessToken = await _tokenService.GetAccessToken(user, jwtTokenId);
            var refreshToken = await _tokenService.CreateNewRefreshToken(user.Id, jwtTokenId);

            return new Tokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
    }
}
