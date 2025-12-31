using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class ProductWishlistDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockAmount { get; set; }
        public string Brand { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
        public string ProductCode { get; set; }
        public decimal AverageRating { get; set; }

    }
}
