using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.ProductImages.Commands.AddProductImage
{
    public class AddProductImageCommandHandler: IRequestHandler<AddProductImageCommand,int>
    {
        private readonly IAppDbContext _context;
        private readonly IImageService _imageService;

        public AddProductImageCommandHandler(IAppDbContext context,IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<int> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
        {
            //if(request.ImageDto.Image is null)
            //{
            //    throw new Exception("No Image Uploaded");
            //}

            //string imageName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageDto.Image.FileName);
            //string imagePath = @"wwwroot/images/" + imageName;
            //var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            //FileInfo file = new FileInfo(directoryLocation);
            //if (file.Exists)
            //{
            //    file.Delete();
            //}
            //using (var stream = new FileStream(directoryLocation, FileMode.Create))
            //{
            //    await request.ImageDto.Image.CopyToAsync(stream);
            //}

            //ProductImage image = new()
            //{
            //    ProductId = request.ProductId,
            //    ImageUrl = request.baseUrl + "/images/" + imageName,
            //    ImageLocalPath = imagePath
            //};

            var paths = await _imageService.UploadImage(request.ImageDto.Image, request.baseUrl);

            ProductImage image = new()
            {
                ProductId = request.ProductId,
                ImageUrl = paths.ImageUrl,
                ImageLocalPath = paths.ImageLocalPath
            };


            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync(cancellationToken);

            return image.ImageId;
        }
    }
}
