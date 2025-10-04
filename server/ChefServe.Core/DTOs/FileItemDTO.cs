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
    public string Token { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string ParentPath { get; set; } = string.Empty;

}