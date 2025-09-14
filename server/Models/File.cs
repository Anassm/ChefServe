using System.ComponentModel.DataAnnotations;

namespace ChefServe.Models;

public class File
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
    public File? ParentFolder { get; set; }
    public ICollection<File> ChildItems { get; set; } = new List<File>();
}