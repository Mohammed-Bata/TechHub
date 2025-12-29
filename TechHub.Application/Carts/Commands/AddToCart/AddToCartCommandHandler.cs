using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Entities;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Carts.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public AddToCartCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException("notfound");
            }


                var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = request.UserId,
                    Items = new List<ShoppingCartItem>()
                };
                _context.Carts.Add(cart);
            }
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
                
            }
            else
            {
                cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = product.TotalPrice
                };
                cart.Items.Add(cartItem);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return cartItem.Id;
        }
    }
}
