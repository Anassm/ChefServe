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
    private readonly IHashService _hashService;

    public AdminController(ISessionService sessionService, IFileService fileService, IUserService userService, IHashService hashService)
    {
        _sessionService = sessionService;
        _fileService = fileService;
        _userService = userService;
        _hashService = hashService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
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

    [HttpGet("users/count")]
    public async Task<IActionResult> GetUserCount()
    {
        var count = await _userService.GetUserCountAsync();
        return Ok(new { userCount = count });
    }

    [HttpGet("files/count")]
    public async Task<IActionResult> GetFileCount()
    {
        var count = await _fileService.GetFileCountAsync();
        return Ok(new { fileCount = count });
    }

    [HttpGet("folders/count")]
    public async Task<IActionResult> GetFolderCount()
    {
        var count = await _fileService.GetFolderCountAsync();
        return Ok(new { folderCount = count });
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
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
        var createdUser = await _userService.CreateUserAsync(newUser);
        return CreatedAtAction(nameof(GetAllUsers), new { id = createdUser.ID }, createdUser);
    }

    [HttpPut("users")]
    public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
    {
        var userToUpdate = await _userService.UpdateUserAsync(updatedUser);
        return Ok(userToUpdate);
    }

}
