using ChefServe.Core.Models;
namespace ChefServe.Core.DTOs;

public class FileItemDTO
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFolder { get; set; } = false;
    public Guid OwnerID { get; set; }
}
public class CreateFolderBodyDTO
{
    public required string Token { get; set; }
    public required string FolderName { get; set; }
    public required string ParentPath { get; set; }
}
public class UploadFileBodyDTO
{
    public required string Token { get; set; }
    public required string FileName { get; set; }
    public required Stream Content { get; set; }
    public required string DestinationPath { get; set; }
}