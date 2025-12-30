using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public record OrderItemDto
    {
        public Guid ItemId { get; set; }
        public Guid ProductId { get; init; }
        public string ProductName { get; set; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }

    }
}
