namespace ChefServe.Core.Models;

public class SharedFileItem
{
    public Guid FileID { get; set; }
    public required FileItem File { get; set; }

    public Guid UserID { get; set; }
    public User? User { get; set; }

    public Permission Permission { get; set; }
}

public enum Permission
{
    Read,
    Write
} 