using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Domain.Exceptions;

namespace TechHub.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler: IRequestHandler<UpdateProductCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public UpdateProductCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.ProductId} not found.");
            }
            product.Name = request.ProductDto.Name;
            product.Description = request.ProductDto.Description;
            product.Price = request.ProductDto.Price;
            product.StockAmount = request.ProductDto.StockAmount;
            product.Brand = request.ProductDto.Brand;
            product.CategoryId = request.ProductDto.CategoryId;
            product.ProductCode = request.ProductDto.ProductCode;


            // Image handling
            if (request.ProductDto.CoverImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldImageFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath));
                    if (oldImageFile.Exists)
                    {
                        oldImageFile.Delete();
                    }
                }

                string filename = Guid.NewGuid().ToString() + Path.GetExtension(request.ProductDto.CoverImage.FileName);
                string imagepath = @"wwwroot\images\" + filename;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagepath);

                using (var filestream = new FileStream(directoryLocation, FileMode.Create))
                {
                    await request.ProductDto.CoverImage.CopyToAsync(filestream);
                }
               
                product.ImageUrl = request.baseUrl + "/images/" + filename;
               product.ImageLocalPath = imagepath;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
