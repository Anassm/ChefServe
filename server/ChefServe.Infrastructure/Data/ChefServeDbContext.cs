using Microsoft.EntityFrameworkCore;
using ChefServe.Core.Models;

namespace YourNamespace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<SharedFileItem> SharedFileItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.ID);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                // SQLite default current timestamp
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Files
            modelBuilder.Entity<FileItem>(entity =>
            {
                entity.HasKey(f => f.ID);
                entity.Property(f => f.Name).IsRequired().HasMaxLength(255);
                entity.Property(f => f.Path).IsRequired();
                entity.Property(f => f.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(f => f.UpdatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(f => f.Owner)
                      .WithMany(u => u.FileItems)
                      .HasForeignKey(f => f.OwnerID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FileShares (junction table)
            modelBuilder.Entity<SharedFileItem>(entity =>
            {
                entity.HasKey(fs => new { fs.FileID, fs.UserID });

                entity.HasOne(fs => fs.File)
                      .WithMany(f => f.SharedWith)
                      .HasForeignKey(fs => fs.FileID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(fs => fs.User)
                      .WithMany(u => u.SharedFileItems)
                      .HasForeignKey(fs => fs.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Enum als string opslaan
                entity.Property(fs => fs.Permission)
                      .HasConversion<string>()    // SQLite kan geen echte enums
                      .HasMaxLength(10)
                      .HasDefaultValue("Read");   // SQLite default literal
            });
        }
    }
}