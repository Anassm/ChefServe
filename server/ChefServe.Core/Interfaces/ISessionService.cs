using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface ISessionService
{
    Task<Session> CreateSessionAsync(string userId, TimeSpan? duration = null, bool invalidateAll = false);
    Task<Session?> GetSessionByTokenAsync(string token);
    Task<bool> InvalidateSessionAsync(Guid ID);
    Task<User?> GetUserBySessionTokenAsync(string token);
    Task<bool> HasActiveSessionsAsync(Guid userId);
}