using ChefServe.Core.Models;
using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using ChefServe.Core.Helper;
using ChefServe.Infrastructure.Data;
using ChefServe.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;
using Microsoft.VisualBasic;
using YamlDotNet.Core.Tokens;



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
        try
        {
            if (createFolderDTO == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Request must contain a body." });

            if (createFolderDTO.Token == null || createFolderDTO.Token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(createFolderDTO.Token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { error = "Invalid token." });

            if (createFolderDTO.FolderName == null || createFolderDTO.FolderName == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Missing folder name." });

            if (createFolderDTO.ParentPath == null)
            {
                createFolderDTO.ParentPath = "";
            }

            var folder = await _fileService.CreateFolderAsync(user.ID, createFolderDTO.FolderName, createFolderDTO.ParentPath);

            if (folder == null)
                return StatusCode(StatusCodes.Status409Conflict, new { error = "Folder could not be created. It may already exist." });

            return StatusCode(StatusCodes.Status200OK, new FileItemDTO
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error.", details = ex.Message });
        }
    }
    [HttpPost("UploadFile")]
    public async Task<ActionResult> UploadFile([FromForm] UploadFileFormDTO uploadFileDTO)
    {
        try
        {
            if (uploadFileDTO == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Request must contain a body." });

            if (uploadFileDTO.Token == null || uploadFileDTO.Token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(uploadFileDTO.Token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (uploadFileDTO.FileName == null || uploadFileDTO.FileName == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file name." });

            if (uploadFileDTO.Content == Stream.Null || uploadFileDTO.Content == null || uploadFileDTO.Content.Length == 0)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing Content" });

            if (uploadFileDTO.DestinationPath == null || uploadFileDTO.DestinationPath == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing destination path" });

            using var stream = uploadFileDTO.Content.OpenReadStream();
            var file = await _fileService.UploadFileAsync(user.ID, uploadFileDTO.FileName, stream, uploadFileDTO.DestinationPath);
            if (file == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "File could not be uploaded." });

            return StatusCode(StatusCodes.Status200OK, new FileItemDTO
            {
                ID = file.ID,
                Name = file.Name,
                Path = file.Path,
                Summary = file.Summary,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt,
                IsFolder = file.IsFolder,
                OwnerID = file.OwnerID
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    [HttpGet("GetFile")]
    public async Task<ActionResult> GetFile([FromQuery] Guid fileID)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (token == null || token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });
            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var file = await _fileService.GetFileAsync(fileID, user.ID);
            if (file == null)
                return StatusCode(StatusCodes.Status204NoContent, new { Error = "File not found" });

            return StatusCode(StatusCodes.Status200OK, new FileItemDTO
            {
                ID = file.ID,
                Name = file.Name,
                Path = file.Path,
                Summary = file.Summary,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt,
                IsFolder = file.IsFolder,
                OwnerID = file.OwnerID
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    [HttpGet("GetFiles")]
    public async Task<ActionResult> GetFiles([FromQuery] string? parentPath)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (token == null || token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });
            
            if (parentPath == null)
                parentPath = string.Empty;

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            var files = await _fileService.GetFilesAsync(user.ID, parentPath);
            if (files == null || files.Count() == 0)
                return StatusCode(StatusCodes.Status204NoContent, new { Error = "File not found" });

            return StatusCode(StatusCodes.Status200OK, files.Select(file => new FileItemDTO
            {
                ID = file.ID,
                Name = file.Name,
                Path = file.Path,
                Summary = file.Summary,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt,
                IsFolder = file.IsFolder,
                OwnerID = file.OwnerID
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    [HttpGet("DownloadFile")]
    public async Task<ActionResult> DownloadFile([FromQuery] Guid fileID)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (token == null || token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var file = await _fileService.GetFileAsync(fileID, user.ID);
            if (file == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found" });

            var stream = await _fileService.DownloadFileAsync(fileID, user.ID);
            if (stream == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found" });

            return File(stream, "application/octet-stream", file.Name, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    [HttpDelete("DeleteFile")]
    public async Task<ActionResult> DeleteFile([FromQuery] Guid fileID)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (token == null || token == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var success = await _fileService.DeleteFileAsync(fileID, user.ID);
            if (!success)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found or could not be deleted." });

            return StatusCode(StatusCodes.Status200OK, new { Message = "File deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
}