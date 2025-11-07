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

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

}