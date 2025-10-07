using ChefServe.Core.Models;
using ChefServe.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChefServe.Infrastructure.Data;

namespace ChefServe.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ChefServeDbContext _context;

    public AuthService(ChefServeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateUserAsync(string username, string password)
    {
        var Query = from u in _context.Users
                    where u.Username == username && u.PasswordHash == password
                    select u;
        return await Query.FirstOrDefaultAsync();
    }

    public async Task<bool> VerifyUserAsync(string username)
    {
        var Query = from u in _context.Users
                    where u.Username == username
                    select u;
        var user = await Query.FirstOrDefaultAsync();
        return user != null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}