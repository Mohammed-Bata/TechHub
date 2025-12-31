using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.ProductImages.Commands.AddProductImage
{
    public class AddProductImageCommandHandler: IRequestHandler<AddProductImageCommand,int>
    {
        private readonly IAppDbContext _context;

        public AddProductImageCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
        {
            if(request.ImageDto is null && request.ImageDto.Image.Length ==0)
            {
                throw new Exception("No Image Uploaded");
            }

            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageDto.Image.FileName);
            string imagePath = @"wwwroot/images/" + imageName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            FileInfo file = new FileInfo(directoryLocation);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(directoryLocation, FileMode.Create))
            {
                await request.ImageDto.Image.CopyToAsync(stream);
            }

            ProductImage image = new()
            {
                ProductId = request.ProductId,
                ImageUrl = request.baseUrl + "/images/" + imageName,
                ImageLocalPath = imagePath
            };

            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync(cancellationToken);

            return image.ImageId;
        }
    }
}
