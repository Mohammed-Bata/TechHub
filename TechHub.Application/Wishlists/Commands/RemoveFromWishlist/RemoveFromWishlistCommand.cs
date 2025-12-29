using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Wishlists.Commands.RemoveFromWishlist
{
    public record RemoveFromWishlistCommand(string UserId,Guid ProductId) : IRequest<Guid>;

}
