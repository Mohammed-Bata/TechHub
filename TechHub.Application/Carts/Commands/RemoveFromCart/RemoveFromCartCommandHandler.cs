using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;

namespace TechHub.Application.Carts.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler: IRequestHandler<RemoveFromCartCommand>
    {
        private readonly IAppDbContext _context;

        public RemoveFromCartCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts.Include(c=>c.Items)
                .Where(c => c.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (cart == null)
            {
                throw new Exception("Cart not found");
            }
            var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId);
            if (item == null)
            {
                throw new Exception("Item not found in cart");
            }
            cart.Items.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            
        }
    }
}
