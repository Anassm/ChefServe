using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChefServe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles([FromQuery] int? parentFolderId = null)
    {
        var userId = GetCurrentUserId();
        var files = await _fileService.GetFilesAsync(userId, parentFolderId);
        
        var fileItems = files.Select(f => new FileItemDto
        {
            Id = f.Id,
            FileName = f.FileName,
            ContentType = f.ContentType,
            FileSize = f.FileSize,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt,
            ParentFolderId = f.ParentFolderId,
            IsFolder = f.IsFolder
        });

        return Ok(fileItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(int id)
    {
        var userId = GetCurrentUserId();
        var file = await _fileService.GetFileAsync(id, userId);
        
        if (file == null)
        {
            return NotFound();
        }

        var fileItem = new FileItemDto
        {
            Id = file.Id,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
            ParentFolderId = file.ParentFolderId,
            IsFolder = file.IsFolder
        };

        return Ok(fileItem);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int? parentFolderId = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var userId = GetCurrentUserId();
        
        using var stream = file.OpenReadStream();
        var uploadedFile = await _fileService.UploadFileAsync(
            file.FileName, 
            stream, 
            file.ContentType, 
            userId, 
            parentFolderId);

        var fileItem = new FileItemDto
        {
            Id = uploadedFile.Id,
            FileName = uploadedFile.FileName,
            ContentType = uploadedFile.ContentType,
            FileSize = uploadedFile.FileSize,
            CreatedAt = uploadedFile.CreatedAt,
            UpdatedAt = uploadedFile.UpdatedAt,
            ParentFolderId = uploadedFile.ParentFolderId,
            IsFolder = uploadedFile.IsFolder
        };

        return Ok(fileItem);
    }

    [HttpPost("create-folder")]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDto createFolderDto)
    {
        if (string.IsNullOrWhiteSpace(createFolderDto.FolderName))
        {
            return BadRequest("Folder name is required");
        }

        var userId = GetCurrentUserId();
        var folder = await _fileService.CreateFolderAsync(
            createFolderDto.FolderName, 
            userId, 
            createFolderDto.ParentFolderId);

        var folderItem = new FileItemDto
        {
            Id = folder.Id,
            FileName = folder.FileName,
            ContentType = folder.ContentType,
            FileSize = folder.FileSize,
            CreatedAt = folder.CreatedAt,
            UpdatedAt = folder.UpdatedAt,
            ParentFolderId = folder.ParentFolderId,
            IsFolder = folder.IsFolder
        };

        return Ok(folderItem);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var userId = GetCurrentUserId();
        var file = await _fileService.GetFileAsync(id, userId);
        
        if (file == null || file.IsFolder)
        {
            return NotFound();
        }

        var fileStream = await _fileService.DownloadFileAsync(id, userId);
        if (fileStream == null)
        {
            return NotFound();
        }

        return File(fileStream, file.ContentType, file.FileName);
    }

    [HttpPut("move")]
    public async Task<IActionResult> MoveFile([FromBody] MoveFileDto moveFileDto)
    {
        var userId = GetCurrentUserId();
        var movedFile = await _fileService.MoveFileAsync(
            moveFileDto.FileId, 
            moveFileDto.NewParentFolderId, 
            userId);

        if (movedFile == null)
        {
            return NotFound();
        }

        var fileItem = new FileItemDto
        {
            Id = movedFile.Id,
            FileName = movedFile.FileName,
            ContentType = movedFile.ContentType,
            FileSize = movedFile.FileSize,
            CreatedAt = movedFile.CreatedAt,
            UpdatedAt = movedFile.UpdatedAt,
            ParentFolderId = movedFile.ParentFolderId,
            IsFolder = movedFile.IsFolder
        };

        return Ok(fileItem);
    }

    [HttpPut("rename")]
    public async Task<IActionResult> RenameFile([FromBody] RenameFileDto renameFileDto)
    {
        var userId = GetCurrentUserId();
        var renamedFile = await _fileService.RenameFileAsync(
            renameFileDto.FileId, 
            renameFileDto.NewName, 
            userId);

        if (renamedFile == null)
        {
            return NotFound();
        }

        var fileItem = new FileItemDto
        {
            Id = renamedFile.Id,
            FileName = renamedFile.FileName,
            ContentType = renamedFile.ContentType,
            FileSize = renamedFile.FileSize,
            CreatedAt = renamedFile.CreatedAt,
            UpdatedAt = renamedFile.UpdatedAt,
            ParentFolderId = renamedFile.ParentFolderId,
            IsFolder = renamedFile.IsFolder
        };

        return Ok(fileItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _fileService.DeleteFileAsync(id, userId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
    }
}