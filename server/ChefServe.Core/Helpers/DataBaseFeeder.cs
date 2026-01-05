using ChefServe.Core.Models;
using ChefServe.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(ChefServeDbContext context)
    {
        if (!context.Users.Any(u => u.Email == "testuser@example.com"))
        {
            var user1 = new User
            {
                ID = Guid.NewGuid(),
                Username = "Test User",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "x5Kssnmg0G7fr4KbTm6mWJYxn+rqlTwh0h/tscQNKQ8=:c+gZvTvTavIKREHwvam8xw==", // :)
                Email = "testuser@example.com",
                CreatedAt = DateTime.UtcNow,
                Role = "user"
            };

            var user2 = new User
            {
                ID = Guid.NewGuid(),
                Username = "Test Admin",
                FirstName = "Test",
                LastName = "Admin",
                PasswordHash = "x5Kssnmg0G7fr4KbTm6mWJYxn+rqlTwh0h/tscQNKQ8=:c+gZvTvTavIKREHwvam8xw==", // :)
                Email = "testadmin@example.com",
                CreatedAt = DateTime.UtcNow,
                Role = "admin"
            };

            context.Users.Add(user1);
            context.Users.Add(user2);

            // Voeg een standaard session toe
            // var session1 = new Session
            // {
            //     ID = Guid.NewGuid(),
            //     Token = "VALID_TEST_TOKEN",
            //     UserID = user1.ID,
            //     CreatedAt = DateTime.UtcNow,
            //     ExpiresAt = DateTime.UtcNow.AddYears(5)
            // };

            // var session2 = new Session
            // {
            //     ID = Guid.NewGuid(),
            //     Token = "VALID_TEST_TOKEN",
            //     UserID = user2.ID,
            //     CreatedAt = DateTime.UtcNow,
            //     ExpiresAt = DateTime.UtcNow.AddYears(5)
            // };

            // context.Sessions.Add(session1);
            // context.Sessions.Add(session2);

            context.SaveChanges();
        }
    }
}
