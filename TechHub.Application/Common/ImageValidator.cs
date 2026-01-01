using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Common
{
    public static class ImageValidator
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly Dictionary<string, byte[]> FileSignatures = new()
        {
            { ".jpg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { ".jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { ".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            { ".webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } }
        };

        public static bool IsValidImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                return false;

            // Validate magic bytes
            using var stream = file.OpenReadStream();
            var header = new byte[8];
            stream.Read(header, 0, header.Length);

            foreach (var signature in FileSignatures.Where(s => s.Key == extension))
            {
                if (header.Take(signature.Value.Length).SequenceEqual(signature.Value))
                    return true;
            }

            return false;
        }
    }
}
