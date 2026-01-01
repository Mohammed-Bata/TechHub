using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Infrastructure.Services
{
    internal class AuthService : IAuthService
    {
        
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AppUser?> FindByEmailAsync(string email)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser == null)
            {
                return null;
            }
            var user = new AppUser
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email,
                DateOfBirth = applicationUser.DateOfBirth
            };
            return user;
        }

        public async Task<AppUser> CheckPasswordAsync(string email, string password)
        {

            var applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser == null)
            {
                return null;
            }
            var result = await _userManager.CheckPasswordAsync(applicationUser, password);

            if (result == false)
            {
                return null;
            }
            return new AppUser
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Email = applicationUser.Email,
                DateOfBirth = applicationUser.DateOfBirth
            };

        }

        public async Task<string> FindEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user.Email;
        }

        
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) == null;
        }
       public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) == null;
        }

        public async Task<bool> CreateUserAsync(AppUser user,string password)
        {
            var applicationUser = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth
            };

            var result = await _userManager.CreateAsync(applicationUser,password);

            if (!result.Succeeded)
            {
                return false;
            }
            await _userManager.AddToRoleAsync(applicationUser, "Customer");

            return true;

        }

       
    }

}
