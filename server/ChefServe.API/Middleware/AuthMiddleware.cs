using ChefServe.Core.Interfaces;
using ChefServe.Core.Models;


namespace ChefServe.API.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
    {
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && path.StartsWith("/api/auth/login"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Missing token." });
            return;
        }

        var user = await sessionService.GetUserBySessionTokenAsync(token);
        if (user == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid token." });
            return;
        }

        context.Items["User"] = user;

        await _next(context);
    }
}

public static class AuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}

public static class HttpContextExtensions
{
    public static User GetUser(this HttpContext context) =>
        (User)context.Items["User"]!;
}

