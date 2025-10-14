using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface ISessionService
{
    Task<Session> CreateSessionAsync(string userId, TimeSpan? duration = null);
    Task<Session?> GetSessionByTokenAsync(string token);
    Task<bool> InvalidateSessionAsync(Guid ID);
    Task<User?> GetUserBySessionTokenAsync(string token);
}