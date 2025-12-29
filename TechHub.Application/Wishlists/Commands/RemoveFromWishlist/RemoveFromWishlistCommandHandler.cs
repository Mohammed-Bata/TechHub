using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;

namespace TechHub.Application.Wishlists.Commands.RemoveFromWishlist
{
    public class RemoveFromWishlistCommandHandler: IRequestHandler<RemoveFromWishlistCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public RemoveFromWishlistCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            var wishlist = _context.Wishlists.FirstOrDefault(w => w.UserId == request.UserId);
            if (wishlist != null)
            {
                var productWishlist = wishlist.Products.FirstOrDefault(wp => wp.ProductId == request.ProductId);
                if (productWishlist != null)
                {
                    wishlist.Products.Remove(productWishlist);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            return wishlist.Id;
        }
    }
}
