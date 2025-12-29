
namespace TechHub.Application.DTOs
{
    public record RegisterationRequest
    (
      string FirstName,
      string LastName,
      string Email,
      string UserName,
      DateTime DateOfBirth,
      string Password,
      string ConfirmPassword
    );
}
