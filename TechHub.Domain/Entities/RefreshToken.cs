using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string JwtTokenId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Refresh_Token { get; set; }
        public bool IsValid { get; set; }
    }
}
