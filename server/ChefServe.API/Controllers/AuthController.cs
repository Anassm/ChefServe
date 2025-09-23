using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChefServe.Core.Models;
using ChefServe.Core.DTOs;

namespace ChefServe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        User user = new()
        {
            Username = registerDto.Username,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(null, registerDto.Password),
        };

        var result = await ...;

        var token = ...;

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {


        return Ok();
    }
}