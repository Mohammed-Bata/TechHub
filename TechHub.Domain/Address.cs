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
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string PostalCode { get; set; }
    }
}
