using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } = 0;
        public int StockAmount { get; set; }
        public string Brand { get; set; }
        public string ProductCode { get; set; }       
        public int CategoryId { get; set; }
        public string? CoverImageUrl { get; set; }
        public IFormFile CoverImage { get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
