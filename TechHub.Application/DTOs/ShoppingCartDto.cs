using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
        public string UserId { get; set; }
    }
}
