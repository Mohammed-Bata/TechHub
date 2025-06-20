using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Domain;

namespace TechHub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueAsync(string email, string userName);
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByIdAsync(string id);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<AppUser> AddUser(AppUser user, string password);
    }
   
}
