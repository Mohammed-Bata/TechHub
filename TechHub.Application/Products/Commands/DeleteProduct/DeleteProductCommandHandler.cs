using MediatR;
using Microsoft.EntityFrameworkCore;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private  IAppDbContext _context;

        public DeleteProductCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
           
            var deletedCount = await _context.Products
                .Where(p => p.Id == request.ProductId)
                .ExecuteDeleteAsync(cancellationToken);
            if (deletedCount == 0)
            {
                throw new NotFoundException("not found");
            }

            return true; 
        }
    }
}
