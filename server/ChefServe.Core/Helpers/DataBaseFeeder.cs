using ChefServe.Core.Models;
using ChefServe.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(ChefServeDbContext context)
    {
        if (!context.Users.Any(u => u.Email == "testuser@example.com"))
        {
            var user = new User
            {
                ID = Guid.NewGuid(),
                Username = "Test User",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "vervang_dit_met_een_gehasht_wachtwoord", 
                Email = "testuser@example.com",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            // Voeg een standaard session toe
            var session = new Session
            {
                ID = Guid.NewGuid(),
                Token = "VALID_TEST_TOKEN",
                UserID = user.ID,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(5)
            };

            context.Sessions.Add(session);

            context.SaveChanges();
        }
    }
}
