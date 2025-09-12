using ChefServe.Core.Entities;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChefServe.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _context;
    private readonly string _uploadPath;

    public FileService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _uploadPath = configuration["FileStorage:UploadPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<FileItem> UploadFileAsync(string fileName, Stream fileStream, string contentType, string userId, int? parentFolderId = null)
    {
        // Generate unique filename to avoid conflicts
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Save file to disk
        using (var fileStreamDisk = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamDisk);
        }

        // Create file record
        var fileItem = new FileItem
        {
            FileName = fileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileStream.Length,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId,
            ParentFolderId = parentFolderId,
            IsFolder = false
        };

        _context.FileItems.Add(fileItem);
        await _context.SaveChangesAsync();

        return fileItem;
    }

    public async Task<FileItem> CreateFolderAsync(string folderName, string userId, int? parentFolderId = null)
    {
        var folder = new FileItem
        {
            FileName = folderName,
            FilePath = string.Empty,
            ContentType = "folder",
            FileSize = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId,
            ParentFolderId = parentFolderId,
            IsFolder = true
        };

        _context.FileItems.Add(folder);
        await _context.SaveChangesAsync();

        return folder;
    }

    public async Task<FileItem?> GetFileAsync(int fileId, string userId)
    {
        return await _context.FileItems
            .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
    }

    public async Task<IEnumerable<FileItem>> GetFilesAsync(string userId, int? parentFolderId = null)
    {
        return await _context.FileItems
            .Where(f => f.UserId == userId && f.ParentFolderId == parentFolderId)
            .OrderBy(f => f.IsFolder ? 0 : 1) // Folders first
            .ThenBy(f => f.FileName)
            .ToListAsync();
    }

    public async Task<Stream?> DownloadFileAsync(int fileId, string userId)
    {
        var fileItem = await GetFileAsync(fileId, userId);
        
        if (fileItem == null || fileItem.IsFolder || !File.Exists(fileItem.FilePath))
        {
            return null;
        }

        return new FileStream(fileItem.FilePath, FileMode.Open, FileAccess.Read);
    }

    public async Task<bool> DeleteFileAsync(int fileId, string userId)
    {
        var fileItem = await GetFileAsync(fileId, userId);
        
        if (fileItem == null)
        {
            return false;
        }

        // If it's a file, delete from disk
        if (!fileItem.IsFolder && File.Exists(fileItem.FilePath))
        {
            File.Delete(fileItem.FilePath);
        }

        // Delete from database (this will cascade delete child items if it's a folder)
        _context.FileItems.Remove(fileItem);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<FileItem?> MoveFileAsync(int fileId, int? newParentFolderId, string userId)
    {
        var fileItem = await GetFileAsync(fileId, userId);
        
        if (fileItem == null)
        {
            return null;
        }

        // Validate that the new parent folder exists and belongs to the user
        if (newParentFolderId.HasValue)
        {
            var parentFolder = await GetFileAsync(newParentFolderId.Value, userId);
            if (parentFolder == null || !parentFolder.IsFolder)
            {
                return null;
            }
        }

        fileItem.ParentFolderId = newParentFolderId;
        fileItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return fileItem;
    }

    public async Task<FileItem?> RenameFileAsync(int fileId, string newName, string userId)
    {
        var fileItem = await GetFileAsync(fileId, userId);
        
        if (fileItem == null || string.IsNullOrWhiteSpace(newName))
        {
            return null;
        }

        fileItem.FileName = newName.Trim();
        fileItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return fileItem;
    }
}