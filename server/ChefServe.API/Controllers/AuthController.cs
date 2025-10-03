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
    // This is only for  hashing password
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAuthService _authService;
    private readonly ISessionService _sessionService;

    public AuthController(IPasswordHasher<User> passwordHasher, IAuthService authService, ISessionService sessionService)
    {

        _passwordHasher = passwordHasher;
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
        Console.WriteLine(_passwordHasher.HashPassword(null, loginDto.Password));

        var user = await _authService.AuthenticateUserAsync(loginDto.Username, _passwordHasher.HashPassword(null, loginDto.Password));

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var session = await _sessionService.CreateSessionAsync(user.ID.ToString(), TimeSpan.FromHours(24));
        return Ok(new AuthResponseDto
        {
            Token = session.Token,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
        });
    }
}
