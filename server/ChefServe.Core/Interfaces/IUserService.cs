using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface IUserService
{
    User GetUserById(Guid userId);
    Task<List<User>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(Guid userId);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
}