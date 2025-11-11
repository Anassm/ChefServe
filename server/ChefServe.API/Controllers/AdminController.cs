using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChefServe.Core.Models;
using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using System.Security.Cryptography;


namespace ChefServe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IFileService _fileService;
    private readonly IUserService _userService;

    public AdminController(ISessionService sessionService, IFileService fileService, IUserService userService)
    {
        _sessionService = sessionService;
        _fileService = fileService;
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (!Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            Console.WriteLine("No authorization token provided.");
            return Unauthorized("No authorization token provided.");
        }

        var session = await _sessionService.GetSessionByTokenAsync(token);
        if (session == null)
        {
            Console.WriteLine("Invalid session token: " + token);
            return Unauthorized("Invalid session token.");
        }

        var user = await _sessionService.GetUserBySessionTokenAsync(token);
        if (user == null || !user.IsAdmin)
        {
            Console.WriteLine("User is not admin: " + (user?.Role ?? "null"));
            return Forbid("User does not have admin privileges.");
        }


        var users = await _userService.GetAllUsersAsync();
        Console.WriteLine("Fetched users: " + string.Join(", ", users.Select(u => u.Email)));
        var simplifiedUsers = users.Select(u => new
        {
            id = u.ID,
            username = u.Username,
            firstName = u.FirstName,
            lastName = u.LastName,
            email = u.Email,
            role = u.Role,
            createdAt = u.CreatedAt
        });
        return Ok(simplifiedUsers);
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        if (!Request.Headers.TryGetValue("Authorization", out var token))
        {
            return Unauthorized("No authorization token provided.");
        }

        var session = await _sessionService.GetSessionByTokenAsync(token);
        if (session == null)
        {
            return Unauthorized("Invalid session token.");
        }

        var user = await _sessionService.GetUserBySessionTokenAsync(token);
        if (user == null || !user.IsAdmin)
        {
            return Forbid("User does not have admin privileges.");
        }

        var result = await _userService.DeleteUserAsync(userId);
        if (!result)
        {
            return NotFound("User not found.");
        }

        return NoContent();
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] User newUser)
    {
        if (!Request.Headers.TryGetValue("Authorization", out var token))
        {
            return Unauthorized("No authorization token provided.");
        }

        var session = await _sessionService.GetSessionByTokenAsync(token);
        if (session == null)
        {
            return Unauthorized("Invalid session token.");
        }

        var user = await _sessionService.GetUserBySessionTokenAsync(token);
        if (user == null || !user.IsAdmin)
        {
            return Forbid("User does not have admin privileges.");
        }

        var createdUser = await _userService.CreateUserAsync(newUser);
        return CreatedAtAction(nameof(GetAllUsers), new { id = createdUser.ID }, createdUser);
    }

    [HttpPut("users")]
    public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
    {
        if (!Request.Headers.TryGetValue("Authorization", out var token))
        {
            return Unauthorized("No authorization token provided.");
        }

        var session = await _sessionService.GetSessionByTokenAsync(token);
        if (session == null)
        {
            return Unauthorized("Invalid session token.");
        }

        var user = await _sessionService.GetUserBySessionTokenAsync(token);
        if (user == null || !user.IsAdmin)
        {
            return Forbid("User does not have admin privileges.");
        }

        var userToUpdate = await _userService.UpdateUserAsync(updatedUser);
        return Ok(userToUpdate);
    }
}
