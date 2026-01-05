namespace ChefServe.Core.Models;

using System.Text.Json.Serialization;

public class SharedFileItem
{
    public Guid FileID { get; set; }

    [JsonIgnore]
    public required FileItem File { get; set; }

    public Guid UserID { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public Permission Permission { get; set; }
}

public enum Permission
{
    Read,
    Write
} 