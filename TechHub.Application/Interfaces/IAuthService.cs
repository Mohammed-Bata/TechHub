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
        Task<AppUser> Register(RegisterationRequest request);
        Task<Tokens> Login(LoginRequest request);
    }
}
