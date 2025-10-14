using ChefServe.Core.Models;
namespace ChefServe.Core.DTOs;

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
    public required string name { get; set; }
    public required string extension { get; set; }
    public required bool isFolder { get; set; }
    public required bool hasContent { get; set; }
}