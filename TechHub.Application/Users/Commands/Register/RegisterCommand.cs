using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Users.Commands.Register
{
    public record RegisterCommand(string FirstName, string LastName, string UserName, string Email, DateTime DateOfBirth, string Password):IRequest<UserDto>;

}
