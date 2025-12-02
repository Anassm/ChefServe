using ChefServe.Core.Models;
namespace ChefServe.Core.DTOs;

public class FileServiceResponseDTO
{
    public required bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}
public class FileItemDTO
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string Extension { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFolder { get; set; } = false;
    public Guid OwnerID { get; set; }
}
public class CreateFolderBodyDTO
{
    public required string FolderName { get; set; }
    public string? ParentPath { get; set; }
}
public class UploadFileFormDTO
{
    public required string FileName { get; set; }
    public required IFormFile Content { get; set; }
    public string? DestinationPath { get; set; }
    public FileConflictMode? ConflictMode { get; set; }
}
public enum FileConflictMode
{
    Overwrite,
    Suffix,
    Cancel
}
public class RenameFileBodyDTO
{
    public required Guid FileID { get; set; }
    public required string NewName { get; set; }
}
public class MoveFileBodyDTO
{
    public required Guid FileID { get; set; }
    public required string NewPath { get; set; }
}
public class getFilesReturnDTO
{
    public required Guid id { get; set; }
    public required string name { get; set; }
    public required string extension { get; set; }
    public required bool isFolder { get; set; }
    public required bool hasContent { get; set; }
}

public class GetFileTreeReturnDTO
{
    public required Guid id { get; set; }
    public required string name { get; set; }
    public required string folderPath { get; set; }
    public required string parentPath { get; set; }
    public required List<GetFileTreeReturnDTO> children { get; set; }
}