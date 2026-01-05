using ChefServe.Core.DTOs;
using ChefServe.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ChefServe.API.Middleware;
using ChefServe.API.Validators;



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
        var user = HttpContext.GetUser();

        var validation = CreateFolderValidator.Validate(createFolderDTO);
        if (!validation.IsValid)
            return BadRequest(new { error = validation.Error });

        FileServiceResponseDTO result = await _fileService.CreateFolderAsync(user.ID, createFolderDTO.FolderName, createFolderDTO.ParentPath);

        FileItemDTO? returnData = null;
        if (result.Data != null)
        {
            dynamic fileData = result.Data;
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
            404 => StatusCode(StatusCodes.Status404NotFound, new { result.Success, result.Message }),
            409 => StatusCode(StatusCodes.Status409Conflict, new { result.Success, result.Message }),
            500 => StatusCode(StatusCodes.Status500InternalServerError, new { result.Success, result.Message }),
            _ => StatusCode(StatusCodes.Status501NotImplemented, new { result.Success, Message = "Not implemented status code." })
        };
    }

    [HttpPost("UploadFile")]
    public async Task<ActionResult> UploadFile([FromForm] UploadFileFormDTO uploadFileDTO)
    {
        try
        {
            var user = HttpContext.GetUser();

            var validation = UploadFileValidator.Validate(uploadFileDTO);
            if (!validation.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = validation.Error });

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
            var user = HttpContext.GetUser();

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
            var user = HttpContext.GetUser();

            if (parentPath == null)
                parentPath = string.Empty;

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
            var user = HttpContext.GetUser();

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

    [HttpGet("ViewFile")]
    public async Task<ActionResult> ViewFile([FromQuery] Guid fileID)
    {
        try
        {
            var user = HttpContext.GetUser();

            if (fileID == Guid.Empty)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = "Missing file ID" });

            var file = await _fileService.GetFileAsync(fileID, user.ID);
            if (file.Data == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found in database" });
            dynamic fileData = file.Data;

            var stream = await _fileService.DownloadFileAsync(fileID, user.ID);
            if (stream == null)
                return StatusCode(StatusCodes.Status404NotFound, new { Error = "File not found on drive" });

            string contentType = GetContentType(fileData.Extension);
            
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileData.Name}\"");
            
            return File(stream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", Details = ex.Message });
        }
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".htm" => "text/html",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".csv" => "text/csv",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream",
        };
    }

    [HttpDelete("DeleteFile")]
    public async Task<ActionResult> DeleteFile([FromQuery] Guid fileID)
    {
        try
        {
            var user = HttpContext.GetUser();

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
            var user = HttpContext.GetUser();

            var validation = RenameFileValidator.Validate(renameFileDTO);
            if (!validation.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, new { Error = validation.Error });

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
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Internal server error.", ex.Message });
        }
    }

    [HttpGet("GetFileTree")]
    public async Task<ActionResult> GetFileTree()
    {
        try
        {
            var user = HttpContext.GetUser();

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