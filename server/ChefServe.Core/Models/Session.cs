namespace ChefServe.Core.Models;

public class Session
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public string UserID { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public User? User { get; set; } 
}