using Microsoft.AspNetCore.Identity;

namespace ChefServe.Core.Models;

public class User
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FileItem> FileItems { get; set; } = new List<FileItem>();
    public ICollection<SharedFileItem> SharedFileItems { get; set; } = new List<SharedFileItem>();
}