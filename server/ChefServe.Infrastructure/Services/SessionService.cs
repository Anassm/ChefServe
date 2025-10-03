using ChefServe.Core.Models;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;
using System.Security.Cryptography;

namespace ChefServe.Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly ChefServeDbContext _context;

    public SessionService(ChefServeDbContext context)
    {
        _context = context;
    }

    public async Task<Session> CreateSessionAsync(string userId, TimeSpan? duration)
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
        string token = Convert.ToBase64String(tokenBytes);

        Session session = new Session
        {
            ID = Guid.NewGuid(),
            Token = token,
            UserID = Guid.Parse(userId),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(duration ?? TimeSpan.FromHours(24))
        };

        var query = await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return query.Entity;


    }

    public async Task<Session?> GetSessionByTokenAsync(string token)
    {
        var session = await _context.Sessions.FindAsync(token);
        return session;
    }

    public async Task<bool> InvalidateSessionAsync(string token)
    {
        var session = await _context.Sessions.FindAsync(token);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<User?> GetUserBySessionTokenAsync(string token)
    {
        var session = await _context.Sessions.FindAsync(token);
        if (session == null || session.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        var user = await _context.Users.FindAsync(session.UserID);
        return user;
    }


}