using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechHub.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal TotalPrice
        {
            get
            {
                return Price - Price * Discount;
            }
        }
        public int StockAmount { get; set; }
        public int Discount { get; set; } = 0;
        public string Brand { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string ImageUrl { get; set; }

        public string ImageLocalPath { get; set; }
        public string ProductCode { get; set; }
        public decimal AverageRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        
    }
}
