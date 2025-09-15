using Microsoft.AspNetCore.Identity;

namespace ChefServe.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<FileItem> Files { get; set; } = new List<FileItem>();
}