using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Carts.Commands.RemoveFromCart
{
    public record RemoveFromCartCommand(string UserId, Guid ItemId) : IRequest;

}
