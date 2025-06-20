using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Domain
{
    public class ProductWishlist
    {
        public int ProductId { get; set; }
       
        public Product Product { get; set; }

        public int WishlistId { get; set; }
       
    }
}
