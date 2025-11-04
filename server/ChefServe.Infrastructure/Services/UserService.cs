using ChefServe.Core.Models;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ChefServe.Core.Interfaces;

namespace ChefServe.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ChefServeDbContext _context;
    public UserService(ChefServeDbContext context)
    {
        _context = context;
    }

    public User GetUserById(Guid userId)
    {
        var user = _context.Users.Find(userId);
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}