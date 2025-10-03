using ChefServe.Core.Models;
using ChefServe.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChefServe.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace ChefServe.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ChefServeDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(ChefServeDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> AuthenticateUserAsync(string username, string password)
    {
        var Query = from u in _context.Users
                    where u.Username == username
                    select u;
        _passwordHasher.VerifyHashedPassword(null, Query.)
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
}