using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Users.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
    {
       
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
           
            _authService = authService;
        }

        public async Task<UserDto> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            //var registerationRequest = new RegisterationRequest
            //(
            //   command.FirstName,
            //    command.LastName,
            //    command.Email,
            //    command.UserName,
            //    command.DateOfBirth,
            //    command.Password,
            //    command.Password
            //);
            //var user = await _authService.Register(registerationRequest);
            //if (user != null)
            //{
            //    return new UserDto
            //    {
            //        FirstName = user.FirstName,
            //        LastName = user.LastName,
            //        UserName = user.UserName,
            //        Email = user.Email,
            //        DateOfBirth = user.DateOfBirth
            //    };
            //}
            //return null;

           
            bool isUnique = await _authService.IsEmailUniqueAsync(command.model.Email) && await _authService.IsUsernameUniqueAsync(command.model.UserName);
            if (!isUnique)
            {
                throw new Exception("email or username is exist");
            }

            var user = new AppUser
            {
                FirstName = command.model.FirstName,
                LastName = command.model.LastName,
                Email = command.model.Email,
                UserName = command.model.UserName,
                DateOfBirth = command.model.DateOfBirth,
            };

            var result = await _authService.CreateUserAsync(user,command.model.Password);

            if (!result)
            {
                throw new Exception("create user failed");
            }

            UserDto userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth,
            };
            return userDto;
        }

    }
}
