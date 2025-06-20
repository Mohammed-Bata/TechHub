using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Domain
{
    public class ProductImage
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }

        public string ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
    }
}
