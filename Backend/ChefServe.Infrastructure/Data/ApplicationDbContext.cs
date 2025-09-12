using ChefServe.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChefServe.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FileItem> FileItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure FileItem entity
        builder.Entity<FileItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.ContentType)
                .IsRequired();
                
            entity.Property(e => e.UserId)
                .IsRequired();

            // Configure self-referencing relationship for folders
            entity.HasOne(e => e.ParentFolder)
                .WithMany(e => e.ChildItems)
                .HasForeignKey(e => e.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with User
            entity.HasOne<ApplicationUser>()
                .WithMany(u => u.Files)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ApplicationUser entity
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName)
                .HasMaxLength(100);
                
            entity.Property(e => e.LastName)
                .HasMaxLength(100);
        });
    }
}