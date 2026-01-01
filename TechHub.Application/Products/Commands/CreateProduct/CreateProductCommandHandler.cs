using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;

namespace TechHub.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IAppDbContext _context;
        private readonly IImageService _imageService;

        public CreateProductCommandHandler(IAppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new TechHub.Domain.Entities.Product
            {
                Name = request.ProductDto.Name,
                Description = request.ProductDto.Description,
                Price = request.ProductDto.Price,
                StockAmount = request.ProductDto.StockAmount,
                Brand = request.ProductDto.Brand,
                CategoryId = request.ProductDto.CategoryId,
                ProductCode = request.ProductDto.ProductCode,
            };

            //Image handling 

            //if (request.ProductDto.CoverImage == null)
            //{
            //    throw new Exception("No Image Uploaded");
            //}


            //string imageName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProductDto.CoverImage.FileName);
            //string imagePath = @"wwwroot/images/" + imageName;
            //var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            //FileInfo file = new FileInfo(directoryLocation);
            //if (file.Exists)
            //{
            //    file.Delete();
            //}
            //using (var stream = new FileStream(directoryLocation, FileMode.Create))
            //{
            //    await request.ProductDto.CoverImage.CopyToAsync(stream);
            //}

            //product.ImageUrl = request.baseUrl + "/images/" + imageName;
            //product.ImageLocalPath = imagePath;

            var paths = await _imageService.UploadImage(request.ProductDto.CoverImage, request.baseUrl);

            product.ImageUrl = paths.ImageUrl;
            product.ImageLocalPath = paths.ImageLocalPath;

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

    }
}
