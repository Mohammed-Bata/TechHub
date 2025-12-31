using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain.Entities;

namespace TechHub.Application.Users.Commands.Refresh
{
    public record RefreshTokensCommand(Tokens tokens):IRequest<Tokens>;
    
}
