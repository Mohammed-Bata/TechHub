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

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(applicationUser, password);

        }

        public async Task<string> FindEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user.Email;
        }

        //public async Task<Tokens> Login(LoginRequest request)
        //{
        //var applicationUser = await _userManager.FindByEmailAsync(request.Email);
        //if (applicationUser == null)
        //{
        //    return null;
        //}
        //var user = new AppUser
        //{
        //    Id = applicationUser.Id,
        //    FirstName = applicationUser.FirstName,
        //    LastName = applicationUser.LastName,
        //    Email = applicationUser.Email,
        //    DateOfBirth = applicationUser.DateOfBirth
        //};


        //var user = await _userRepository.GetByEmailAsync(request.Email);
        //if (user == null)
        //{
        //    return new Tokens();
        //}
        //var isValid = await _userManager.CheckPasswordAsync(applicationUser, request.Password);
        //var isValid = await _userRepository.CheckPasswordAsync(user, request.Password);
        //if (!isValid)
        //{
        //    return new Tokens();
        //}

        //    var jwtTokenId = Guid.NewGuid().ToString();
        //    var accessToken = await _tokenService.GetAccessToken(user, jwtTokenId);
        //    var refreshToken = await _tokenService.CreateNewRefreshToken(user.Id, jwtTokenId);

        //    Tokens tokens = new()
        //    {
        //        AccessToken = accessToken,
        //        RefreshToken = refreshToken
        //    };
        //    return tokens;

        //}


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

        //public async Task<AppUser> Register(RegisterationRequest request)
        //{
        //    var user = new AppUser
        //    {
        //        FirstName = request.FirstName,
        //        LastName = request.LastName,
        //        Email = request.Email,
        //        DateOfBirth = request.DateOfBirth,
        //        UserName = request.UserName
        //    };
        //    var result = await _userManager.FindByEmailAsync(request.Email) == null &&
        //                 await _userManager.FindByNameAsync(request.UserName) == null;

        //    if (result)
        //    {
        //        var applicationUser = new ApplicationUser
        //        {
        //            FirstName = user.FirstName,
        //            LastName = user.LastName,
        //            Email = user.Email,
        //            UserName = user.UserName,
        //            DateOfBirth = user.DateOfBirth
        //        };

        //       var result2 = await _userManager.CreateAsync(applicationUser, request.Password);

        //        if (!result2.Succeeded)
        //        {
        //            throw new Exception("User creation failed: " + string.Join(", ", result2.Errors.Select(e => e.Description)));
        //        }

        //        //user = await _userRepository.AddUser(user, request.Password);
        //        if (user == null)
        //        {
        //            throw new Exception("User creation failed");
        //        }
        //        else
        //        {
        //            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
        //            {
        //                await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //                await _roleManager.CreateAsync(new IdentityRole("Customer"));
        //            }
        //          applicationUser = await _userManager.FindByEmailAsync(user.Email);
        //            if (applicationUser != null)
        //            {
        //                await _userManager.AddToRoleAsync(applicationUser, "Customer");
        //            }
        //            return user;
        //        }
                   
        //    }
        //    return new AppUser();
        //}
    }

}
