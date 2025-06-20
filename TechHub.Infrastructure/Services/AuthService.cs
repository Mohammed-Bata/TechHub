using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain;

namespace TechHub.Infrastructure.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(IUserRepository userRepository, ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task<Tokens> Login(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return new Tokens();
            }
            var isValid = await _userRepository.CheckPasswordAsync(user, request.Password);
            if (!isValid)
            {
                return new Tokens();
            }
            var jwtTokenId = Guid.NewGuid().ToString();
            var accessToken = await _tokenService.GetAccessToken(user, jwtTokenId);
            var refreshToken = await _tokenService.CreateNewRefreshToken(user.Id, jwtTokenId);

            Tokens tokens = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokens;

        }

        public async Task<AppUser> Register(RegisterationRequest request)
        {
            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                UserName = request.UserName
            };
            var result = await _userRepository.IsUniqueAsync(request.Email, request.UserName);

            if (result)
            {
                user = await _userRepository.AddUser(user, request.Password);
                if (user == null)
                {
                    throw new Exception("User creation failed");
                }
                else
                {
                    if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }
                    var applicationUser = await _userManager.FindByEmailAsync(user.Email);
                    if (applicationUser != null)
                    {
                        await _userManager.AddToRoleAsync(applicationUser, "Customer");
                    }
                    return user;
                }
                   
            }
            return new AppUser();
        }
    }

}
