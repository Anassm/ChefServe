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

            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { error = "Invalid token." });

            if (createFolderDTO.FolderName == null || createFolderDTO.FolderName == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Missing folder name." });

            if (createFolderDTO.ParentPath == null)
            {
                createFolderDTO.ParentPath = "";
            }

            FileServiceResponseDTO result = await _fileService.CreateFolderAsync(user.ID, createFolderDTO.FolderName, createFolderDTO.ParentPath);
            dynamic fileData = result.Data;
            FileItemDTO? returnData = null;
            if (fileData != null)
            {
                returnData = new FileItemDTO
                {
                    ID = fileData.ID,
                    Name = fileData.Name,
                    Path = fileData.Path,
                    Extension = fileData.Extension,
                    Summary = fileData.Summary,
                    CreatedAt = fileData.CreatedAt,
                    UpdatedAt = fileData.UpdatedAt,
                    IsFolder = fileData.IsFolder,
                    OwnerID = fileData.OwnerID
                };
            }

            return result.StatusCode switch
            {
                201 => StatusCode(StatusCodes.Status200OK, new { result.Success, result.Message, returnData }),
                404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
                409 => StatusCode(StatusCodes.Status409Conflict, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, error = "Internal server error.", details = ex.Message });
        }
    }
    [HttpPost("UploadFile")]
    public async Task<ActionResult> UploadFile([FromForm] UploadFileFormDTO uploadFileDTO)
    {
        try
        {
            if (uploadFileDTO == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Request must contain a body." });

            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (uploadFileDTO.FileName == null || uploadFileDTO.FileName == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file name." });

            if (uploadFileDTO.Content == Stream.Null || uploadFileDTO.Content == null || uploadFileDTO.Content.Length == 0)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing Content" });

            if (uploadFileDTO.DestinationPath == null || uploadFileDTO.DestinationPath == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing destination path" });

            using var stream = uploadFileDTO.Content.OpenReadStream();
            var result = await _fileService.UploadFileAsync(user.ID, uploadFileDTO.FileName, stream, uploadFileDTO.DestinationPath, uploadFileDTO.ConflictMode!);
            dynamic fileData = result.Data;
            FileItemDTO? returnData = null;
            if (fileData != null)
            {
                returnData = new FileItemDTO
                {
                    ID = fileData.ID,
                    Name = fileData.Name,
                    Path = fileData.Path,
                    Extension = fileData.Extension,
                    Summary = fileData.Summary,
                    CreatedAt = fileData.CreatedAt,
                    UpdatedAt = fileData.UpdatedAt,
                    IsFolder = fileData.IsFolder,
                    OwnerID = fileData.OwnerID
                };
            }
            return result.StatusCode switch
            {
                201 => StatusCode(StatusCodes.Status201Created, new { result.Success, result.Message, returnData }),
                204 => StatusCode(StatusCodes.Status204NoContent, new { result.Success, result.Message }),
                400 => StatusCode(StatusCodes.Status400BadRequest, new { result.Success, result.Message }),
                404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
                409 => StatusCode(StatusCodes.Status409Conflict, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
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
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });
            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var result = await _fileService.GetFileAsync(fileID, user.ID);
            dynamic fileData = result.Data;
            FileItemDTO? returnData = null;
            if (fileData != null)
            {
                returnData = new FileItemDTO
                {
                    ID = fileData.ID,
                    Name = fileData.Name,
                    Path = fileData.Path,
                    Extension = fileData.Extension,
                    Summary = fileData.Summary,
                    CreatedAt = fileData.CreatedAt,
                    UpdatedAt = fileData.UpdatedAt,
                    IsFolder = fileData.IsFolder,
                    OwnerID = fileData.OwnerID
                };
            }
            return result.StatusCode switch
            {
                200 => StatusCode(StatusCodes.Status200OK, new { result.Success, result.Message, returnData }),
                400 => StatusCode(StatusCodes.Status400BadRequest, new { result.Success, result.Message }),
                404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
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
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            if (parentPath == null)
                parentPath = string.Empty;

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            var result = await _fileService.GetFilesAsync(user.ID, parentPath);
            dynamic filesData = result.Data;
            List<getFilesReturnDTO> returnData = new List<getFilesReturnDTO>();
            if (filesData != null)
            {
                foreach (var file in filesData)
                {
                    returnData.Add(new getFilesReturnDTO
                    {
                        id = file.ID,
                        name = file.Name,
                        extension = file.Extension,
                        isFolder = file.IsFolder,
                        path = file.Path.Substring(46),
                        hasContent = file.HasContent
                    });
                }
            }
            return result.StatusCode switch
            {
                200 => StatusCode(StatusCodes.Status200OK, new { result.Success, result.Message, returnData }),
                204 => StatusCode(StatusCodes.Status204NoContent, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
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
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var file = await _fileService.GetFileAsync(fileID, user.ID);
            if (file.Data == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found in database" });
            dynamic fileData = file.Data;

            var stream = await _fileService.DownloadFileAsync(fileID, user.ID);
            if (stream == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found on drive" });

            return File(stream, "application/octet-stream", fileData.Name, enableRangeProcessing: true);
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
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var result = await _fileService.DeleteFileAsync(fileID, user.ID);

            return result.StatusCode switch
            {
                200 => StatusCode(StatusCodes.Status200OK, new { result.Success, result.Message }),
                404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    [HttpPut("RenameFile")]
    public async Task<ActionResult> RenameFile([FromBody] RenameFileBodyDTO renameFileDTO)
    {
        try
        {
            if (renameFileDTO == null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Request must contain a body." });

            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            if (renameFileDTO.FileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            if (renameFileDTO.NewName == null || renameFileDTO.NewName == string.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing new name." });

            var result = await _fileService.RenameFileAsync(renameFileDTO.FileID, renameFileDTO.NewName, user.ID);

            return result.StatusCode switch
            {
                200 => StatusCode(StatusCodes.Status200OK, new { result.Success, result.Message, result.Data }),
                400 => StatusCode(StatusCodes.Status400BadRequest, new { result.Success, result.Message }),
                404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
                409 => StatusCode(StatusCodes.Status409Conflict, new { result.Success, result.Message }),
                500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
                _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
            };
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
    // [HttpPut("MoveFile")]
    // public async Task<ActionResult> MoveFile([FromBody] MoveFileBodyDTO moveFileDTO)
    // {
    //     try
    //     {
    //         if (!Request.Cookies.TryGetValue("AuthToken", out var token))
    //             return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

    //         var user = await _sessionService.GetUserBySessionTokenAsync(token);
    //         if (user == null)
    //             return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

    //         if (moveFileDTO.FileID == Guid.Empty)
    //             return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

    //         if (moveFileDTO.NewPath == null || moveFileDTO.NewPath == string.Empty)
    //             return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing new path." });

    //         var file = await _fileService.MoveFileAsync(moveFileDTO.FileID, moveFileDTO.NewPath, user.ID);
    //         if (file == null)
    //             return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found or could not be moved." });

    //         return StatusCode(StatusCodes.Status200OK, new FileItemDTO
    //         {
    //             ID = file.ID,
    //             Name = file.Name,
    //             Path = file.Path,
    //             Extension = file.Extension,
    //             Summary = file.Summary,
    //             CreatedAt = file.CreatedAt,
    //             UpdatedAt = file.UpdatedAt,
    //             IsFolder = file.IsFolder,
    //             OwnerID = file.OwnerID
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
    //     }
    // }

    [HttpGet("GetFileTree")]
    public async Task<ActionResult> GetFileTree()
    {
        try
        {
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            var fileTree = await _fileService.GetFileTreeAsync(user.ID);
            if (fileTree == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File tree not found." });

            return StatusCode(StatusCodes.Status200OK, fileTree);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }

    [HttpGet("GetFileInfo")]
    public async Task<ActionResult> GetFileInfo([FromQuery] Guid fileID)
    {
        try
        {
            if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing token." });

            var user = await _sessionService.GetUserBySessionTokenAsync(token);
            if (user == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Error = "Invalid token." });

            var fileInfo = await _fileService.GetFileInfoAsync(user.ID, fileID);
            if (fileInfo == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File info not found." });

            return StatusCode(StatusCodes.Status200OK, fileInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }
}