using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreateProductCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new TechHub.Domain.Entities.Product
            {
                Name = request.productDto.Name,
                Description = request.productDto.Description,
                Price = request.productDto.Price,
                StockAmount = request.productDto.StockAmount,
                Brand = request.productDto.Brand,
                CategoryId = request.productDto.CategoryId,
                ProductCode = request.productDto.ProductCode,
            };

            //Image handling 
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(request.productDto.CoverImage.FileName);
            string imagePath = @"wwwroot/images/" + imageName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            FileInfo file = new FileInfo(directoryLocation);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(directoryLocation, FileMode.Create))
            {
                await request.productDto.CoverImage.CopyToAsync(stream);
            }
           
            product.ImageUrl = request.baseUrl + "/images/" + imageName;
            product.ImageLocalPath = imagePath;


            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product.Id;
        }


    }
}
