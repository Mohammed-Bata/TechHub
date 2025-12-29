using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Carts.Queries.GetCart
{
    public record GetCartQuery(string UserId): IRequest<ShoppingCartDto>;

}
