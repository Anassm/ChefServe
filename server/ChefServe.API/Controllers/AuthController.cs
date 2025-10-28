using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChefServe.Core.Models;
using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using System.Security.Cryptography;


namespace ChefServe.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHashService _hashService;
    private readonly IAuthService _authService;
    private readonly ISessionService _sessionService;

    public AuthController(IHashService hashService, IAuthService authService, ISessionService sessionService)
    {

        _hashService = hashService;
        _authService = authService;
        _sessionService = sessionService;
    }

    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    // {
    //     User user = new()
    //     {
    //         Username = registerDto.Username,
    //         FirstName = registerDto.FirstName,
    //         LastName = registerDto.LastName,
    //         Email = registerDto.Email,
    //         PasswordHash = _passwordHasher.HashPassword(null, registerDto.Password),
    //     };

    //     // Ignore this for now
    //     var result = await ...;

    //     byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
    //     string token = Convert.ToBase64String(tokenBytes);

    //     Session session = new Session
    //     {
    //         ID = Guid.NewGuid(),
    //         Token = token,
    //         UserID = user.ID,
    //         CreatedAt = DateTime.UtcNow,
    //         ExpiresAt = DateTime.UtcNow.AddHours(24)
    //     };

    //     return Ok(new AuthResponseDto
    //     {
    //         Token = token,
    //         Username = user.Username,
    //         FirstName = user.FirstName,
    //         LastName = user.LastName,
    //     });
    // }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        Console.WriteLine("Login attempt for user: " + loginDto.Username);
        Console.WriteLine("Password: " + loginDto.Password);
        Console.WriteLine("Cookies: " + string.Join(", ", Request.Cookies.Select(c => c.Key + "=" + c.Value)));
        if (Request.Cookies.TryGetValue("AuthToken", out var existingToken))
        {
            Console.WriteLine("Existing AuthToken found: " + existingToken);
            var existingSession = await _sessionService.GetSessionByTokenAsync(existingToken);
            if (existingSession != null && existingSession.ExpiresAt > DateTime.UtcNow)
            {
                return Unauthorized("Already logged in.");
            }
        }

        if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Username and password are required.");
        }

        Console.WriteLine(_hashService.ComputeHash(loginDto.Password));
        var user = await _authService.GetUserByUsernameAsync(loginDto.Username);

        if (user == null || !_hashService.VerifyHash(loginDto.Password, user.PasswordHash))
            return Unauthorized("Invalid username or password.");

        // Check for existing active sessions

        Console.WriteLine(loginDto.InvalidateAll);
        bool hasActiveSessions = await _sessionService.HasActiveSessionsAsync(user.ID);
        if (hasActiveSessions && !loginDto.InvalidateAll)
        {
            return Conflict("User already has an active session.");
        }

        Console.WriteLine(user.ID.ToString());
        var session = await _sessionService.CreateSessionAsync(
            user.ID.ToString(),
            TimeSpan.FromHours(24),
            invalidateAll: true
        );


        Response.Cookies.Append("AuthToken", session.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = session.ExpiresAt
        });

        return Ok(new AuthResponseDto
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            return BadRequest("No auth token found.");
        }

        var session = await _sessionService.GetSessionByTokenAsync(token);
        if (session == null)
        {
            return BadRequest("Invalid auth token.");
        }

        await _sessionService.InvalidateSessionAsync(session.ID);

        Response.Cookies.Append("AuthToken", "", new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict
        });

        return Ok("Logged out successfully.");
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (!Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            return Unauthorized("No auth token found.");
        }

        var user = await _sessionService.GetUserBySessionTokenAsync(token);
        if (user == null)
        {
            return Unauthorized("Invalid or expired auth token.");
        }

        return Ok(new AuthResponseDto
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role
        });
    }
}
