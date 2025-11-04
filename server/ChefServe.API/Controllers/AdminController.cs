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
        

        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
