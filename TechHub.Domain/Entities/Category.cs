using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechHub.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
    }
}
