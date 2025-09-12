using Microsoft.AspNetCore.Identity;

namespace ChefServe.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<FileItem> Files { get; set; } = new List<FileItem>();
}