namespace ChefServe.Core.Models;

public class Session
{
    public required Guid ID { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public Guid UserID { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User User { get; set; } = null!;
}