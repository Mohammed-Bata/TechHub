﻿
namespace TechHub.Application.DTOs
{
    public class RegisterationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
