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
        Console.WriteLine(_hashService.ComputeHash(loginDto.Password));
        var user = await _authService.GetUserByUsernameAsync(loginDto.Username);


        if (user == null || !_hashService.VerifyHash(loginDto.Password, user.PasswordHash))
            return Unauthorized("Invalid username or password.");

        Console.WriteLine(user.ID.ToString());
        var session = await _sessionService.CreateSessionAsync(user.ID.ToString(), TimeSpan.FromHours(24));

        Response.Cookies.Append("AuthToken", session.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
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
}
