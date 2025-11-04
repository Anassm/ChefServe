using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface IUserService
{
    User GetUserById(Guid userId);
    Task<List<User>> GetAllUsersAsync();
}