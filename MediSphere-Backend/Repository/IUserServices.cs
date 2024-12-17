using MediSphere.Dtos;
using MediSphere.Models;

namespace MediSphere.Repository
{
    public interface IUserServices
    {
        Task<User?> GetUserByUsernameAsync(string username); // Fetch user by username
        Task<bool> ValidateUserCredentialsAsync(string username, string password); // Validate username and password
        Task<bool> RegisterUserAsync(string username, string password, string role, string email); // Register a new user
        Task<AuthResponseDto> AuthenticateAsync(string username, string password, string email);

        Task<User> ForgotPassword(string emailAddress);
    }
}
