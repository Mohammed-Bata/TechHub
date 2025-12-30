using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

    public record ProductResponseDto {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; } = 0;
        public int StockAmount { get; init; }
        public string Brand { get; init; }
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }
        public string ImageUrl { get; init; }
        public string ProductCode { get; init; }
        public decimal AverageRating { get; init; }

        [JsonIgnore]
        public string? ImageUrlsString { get; init; }
        public  List<string> ImageUrls=> string.IsNullOrEmpty(ImageUrlsString)
            ? new List<string>()
            : ImageUrlsString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
