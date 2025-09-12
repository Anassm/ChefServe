using System.ComponentModel.DataAnnotations;

namespace ChefServe.Core.Entities;

public class FileItem
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    [Required]
    public string ContentType { get; set; } = string.Empty;
    
    public long FileSize { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public int? ParentFolderId { get; set; }
    
    public bool IsFolder { get; set; }
    
    // Navigation properties
    public FileItem? ParentFolder { get; set; }
    public ICollection<FileItem> ChildItems { get; set; } = new List<FileItem>();
}