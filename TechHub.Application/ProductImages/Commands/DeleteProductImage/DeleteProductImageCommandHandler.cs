using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.ProductImages.Commands.DeleteProductImage
{
    public class DeleteProductImageCommandHandler: IRequestHandler<DeleteProductImageCommand>
    {
        private readonly IAppDbContext _context;
        public DeleteProductImageCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            var image = await _context.ProductImages.FindAsync(new object[] { request.ImageId }, cancellationToken);

            if(image == null)
            {
                throw new NotFoundException($"Product image with ID {request.ImageId} not found.");
            }

            var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), image.ImageLocalPath);
            FileInfo file = new FileInfo(oldFilePathDirectory);

            if (file.Exists)
            {
                file.Delete();
            }
             _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}
