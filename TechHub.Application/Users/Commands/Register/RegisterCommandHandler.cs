using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;

namespace TechHub.Application.Users.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<UserDto> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var registerationRequest = new RegisterationRequest
            (
               command.FirstName,
                command.LastName,
                command.Email,
                command.UserName,
                command.DateOfBirth,
                command.Password,
                command.Password
            );
            var user = await _authService.Register(registerationRequest);
            if (user != null)
            {
                return new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth
                };
            }
            return null;
        }




    }
}
