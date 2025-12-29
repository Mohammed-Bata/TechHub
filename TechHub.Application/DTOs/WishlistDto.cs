using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class WishlistDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Default Wishlist";
        public DateTime CreatedAt { get; set; }
        public List<ProductWishlistDto> Products { get; set; }

    }
}
