using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Domain.Entities;

namespace TechHub.Application.Interfaces
{
    public interface IAuthService
    {
        //Task<AppUser> Register(RegisterationRequest request);
        //Task<Tokens> Login(LoginRequest request);

        Task<AppUser?> FindByEmailAsync(string email);
        Task<string> FindEmailByIdAsync(string userId);
        Task<AppUser?> CheckPasswordAsync(string email, string password);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> CreateUserAsync(AppUser user,string password);

    }
}
