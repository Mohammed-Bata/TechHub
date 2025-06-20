using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;
using TechHub.Domain;

namespace TechHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context,UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<bool> IsUniqueAsync(string email, string userName)
        {
            var userByEmail = await _userManager.FindByEmailAsync(email);
            if (userByEmail == null)
            {
                var userByUserName = await _userManager.FindByNameAsync(userName);
                if (userByUserName == null)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<AppUser> GetByEmailAsync(string email)
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
        public async Task<AppUser> GetByIdAsync(string id)
        {
            var applicationUser = await _userManager.FindByIdAsync(id);
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
        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            return appUser != null && await _userManager.CheckPasswordAsync(appUser, password);
        }
        public async Task<AppUser> AddUser(AppUser user,string password)
        {
            var applicationUser = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth
            };
            var result = await _userManager.CreateAsync(applicationUser, password);
            if (result.Succeeded)
            {
                return user;
            }
            else
            {
                return null;
            }
           
        }
    }
    
}
