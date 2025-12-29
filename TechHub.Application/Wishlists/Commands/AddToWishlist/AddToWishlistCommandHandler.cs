using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Wishlists.Commands.AddToWishlist
{
    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public AddToWishlistCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var wishlist = _context.Wishlists.FirstOrDefault(w => w.UserId == request.UserId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Name = "My Wishlist",
                    CreatedAt = DateTime.UtcNow,
                    Products = new List<ProductWishlist>()
                };
                _context.Wishlists.Add(wishlist);
            }
            if (!wishlist.Products.Any(wp => wp.ProductId == request.ProductId))
            {
                wishlist.Products.Add(new ProductWishlist
                {
                    WishlistId = wishlist.Id,
                    ProductId = request.ProductId
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
            return wishlist.Id;
        }
    }
}
