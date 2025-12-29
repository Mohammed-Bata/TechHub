using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Carts.Commands.AddToCart
{
    public record AddToCartCommand(string UserId,Guid ProductId, int Quantity): IRequest<Guid>;
    
}
