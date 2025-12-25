using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Domain.Entities
{
    public class ProductWishlist
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid WishlistId { get; set; }
       
    }
}
