using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.DTOs
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; } 
        public object Data { get; set; }
    }
}
