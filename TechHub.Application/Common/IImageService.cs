using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Common
{
    public interface IImageService
    {
        Task<UploadImageResult> UploadImage(IFormFile Image,string baseUrl);
    }
}
