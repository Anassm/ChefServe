namespace ChefServe.Core.Models;

public class FileItem
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsFolder { get; set; } = false;

    public Guid OwnerID { get; set; }
    public required User Owner { get; set; }

    public ICollection<SharedFileItem> SharedWith { get; set; } = new List<SharedFileItem>();
}