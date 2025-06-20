using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Application.Interfaces
{
    public interface ITokenService
    {
        
        Task<string> GetAccessToken(AppUser user, string jwtTokenId);
        Task<string> CreateNewRefreshToken(string userId, string jwtTokenId);
        Task<Tokens> RefreshAccessToken(Tokens tokens);
        Task RevokeRefreshToken(Tokens model);
    }
}
