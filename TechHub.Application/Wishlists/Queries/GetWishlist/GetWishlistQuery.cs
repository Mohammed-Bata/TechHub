using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;

namespace TechHub.Application.Wishlists.Queries.GetWishlist
{
    public record GetWishlistQuery(string UserId) : IRequest<WishlistDto>;

}
