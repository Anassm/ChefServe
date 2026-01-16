using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ChefServe.Core.Interfaces;

namespace ChefServe.API.Middleware
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
        {
            
            Console.WriteLine("Admin middleware triggered for path: " + context.Request.Path.Value);
            var path = context.Request.Path.Value?.ToLower();

            if (path != null && path.StartsWith("/admin"))
            {
                if (!context.Request.Cookies.TryGetValue("AuthToken", out var token))
                {
                    Console.WriteLine("No AuthToken, returning 401");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("No authorization token provided.");
                    return;
                }

                var session = await sessionService.GetSessionByTokenAsync(token);
                if (session == null)
                {
                    Console.WriteLine("Invalid session, returning 401");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid session token.");
                    return;
                }

                var user = await sessionService.GetUserBySessionTokenAsync(token);
                if (user == null || !user.IsAdmin)
                {
                    Console.WriteLine("User not admin, returning 403");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("User does not have admin privileges.");
                    return;
                }

                context.Items["CurrentUser"] = user;
            }

            if (path != null && path.StartsWith("/api/admin"))
            {
                if (!context.Request.Cookies.TryGetValue("AuthToken", out var token))
                {
                    Console.WriteLine("No AuthToken, returning 401");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("No authorization token provided.");
                    return;
                }

                var session = await sessionService.GetSessionByTokenAsync(token);
                if (session == null)
                {
                    Console.WriteLine("Invalid session, returning 401");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid session token.");
                    return;
                }

                var user = await sessionService.GetUserBySessionTokenAsync(token);
                if (user == null || !user.IsAdmin)
                {
                    Console.WriteLine("User not admin, returning 403");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("User does not have admin privileges.");
                    return;
                }

                context.Items["CurrentUser"] = user;
            }
            Console.WriteLine("Admin middleware passed, proceeding to next middleware.");
            await _next(context);
        }
    }

    public static class AdminAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthMiddleware>();
        }
    }
}