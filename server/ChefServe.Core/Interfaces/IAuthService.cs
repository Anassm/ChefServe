using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateUserAsync(string username, string password);
    Task<bool> VerifyUserAsync(string username);
    Task<User?> GetUserByUsernameAsync(string username);
}