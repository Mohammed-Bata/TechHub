using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Domain.Entities
{
    public class Order
    {
       
        public Guid Id { get; set; }
        public int? AddressId { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string? PaymentIntentId { get; set; }
        public string UserId { get; set; }
      
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
