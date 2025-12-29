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
        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Tokens> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var loginRequest = new LoginRequest
            (
                command.Email,
                command.Password
            );
            var tokens = await _authService.Login(loginRequest);

            if (tokens is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return tokens;
        }
    }
}
