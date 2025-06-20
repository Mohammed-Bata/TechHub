using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class ProductImageDto
    {    
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
