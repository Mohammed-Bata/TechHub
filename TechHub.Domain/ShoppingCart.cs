using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechHub.Domain
{
    public class ShoppingCart
    {
  
        public int Id { get; set; }
        public decimal Price
        {
            get
            {
                decimal totalPrice = 0;
                if (Items is not null)
                    foreach (var item in Items)
                        totalPrice += item.TotalPrice;

                return totalPrice;
            }
        }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public string UserId { get; set; }
       
    }
}
