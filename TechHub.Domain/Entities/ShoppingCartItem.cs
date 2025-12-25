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
    public class ShoppingCartItem
    {
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
