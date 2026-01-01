using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common;

namespace TechHub.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public async Task<UploadImageResult> UploadImage(IFormFile Image, string baseUrl)
        {
            if (Image is null)
            {
                throw new Exception("No Image Uploaded");
            }

            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            string imagePath = @"wwwroot/images/" + imageName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
            FileInfo file = new FileInfo(directoryLocation);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(directoryLocation, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }

            string ImageUrl = baseUrl + "/images/" + imageName;
            string ImageLocalPath = imagePath;

            return new UploadImageResult(ImageUrl,ImageLocalPath);
        }
    }
}
