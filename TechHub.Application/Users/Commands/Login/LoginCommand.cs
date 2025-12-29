using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Application.Users.Commands.Login
{
   public record LoginCommand(
        string Email,
        string Password
        ):IRequest<Tokens>
        ;

}
