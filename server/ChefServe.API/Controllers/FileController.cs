using ChefServe.Core.Models;
using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using ChefServe.Core.Helper;
using ChefServe.Infrastructure.Data;
using ChefServe.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IFileService _fileService;
    public FileController(ISessionService sessionService, IFileService fileService)
    {
        _sessionService = sessionService;
        _fileService = fileService;
    }

    [HttpPost("CreateFolder")]
    public async Task<ActionResult> CreateFolder([FromBody] CreateFolderBodyDTO createFolderDTO)
    {
        
        if (createFolderDTO == null)
            return BadRequest(new { error = "Request body is required." });

        if (string.IsNullOrWhiteSpace(createFolderDTO.Token))
            return BadRequest(new { error = "Token is required." });

        var user = await _sessionService.GetUserBySessionTokenAsync(createFolderDTO.Token);
        if (user == null)
            return Unauthorized(new {error = "Invalid or expired token."});

        if (string.IsNullOrWhiteSpace(createFolderDTO.FolderName))
            return BadRequest(new { error = "Folder name is required." });

        var folder = await _fileService.CreateFolderAsync(user.ID, createFolderDTO.FolderName, createFolderDTO.ParentPath);

        if (folder == null)
            return Conflict(new { error = "Folder could not be created. It may already exist." });

        return Ok(new FileItemDTO
        {
            ID = folder.ID,
            Name = folder.Name,
            Path = folder.Path,
            Summary = folder.Summary,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.UpdatedAt,
            IsFolder = folder.IsFolder,
            OwnerID = folder.OwnerID
        });
    }
}