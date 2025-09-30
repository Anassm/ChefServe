using ChefServe.Core.Models;

public interface ISessionService
{
    Task<Session> CreateSessionAsync(string userId, TimeSpan? duration = null);
    Task<Session?> GetSessionByTokenAsync(string token);
    Task InvalidateSessionAsync(string token);
}