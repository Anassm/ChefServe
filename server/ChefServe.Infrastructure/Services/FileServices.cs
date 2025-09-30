using ChefServe.Core.DTOs;
using ChefServe.Core.Models;
using ChefServe.Core.Helper;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;

public class FileServices : IFileService
{
    private readonly ChefServeDbContext _context;
    public FileServices(ChefServeDbContext context)
    {
        _context = context;
    }

    public async Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
        return null;
    }

    public async Task<FileItem> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath)
    {
        //checks
        if (ownerId == Guid.Empty)
            return null;

        if (fileName == null || fileName.Trim() == string.Empty)
            return null;

        if (content == null || content.Length == 0)
            return null;

        if (destinationPath == null || destinationPath.Trim() == string.Empty)
            return null;

        //create directory
        var fullPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), destinationPath, fileName);
        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream);
        }

        var FileInfo = new FileInfo(fullPath);
        var User = _context.Users.Find(ownerId);
        if (User == null)
            return null;

        return new FileItem
        {
            Name = FileInfo.Name,
            Path = fullPath,
            OwnerID = ownerId,
            CreatedAt = FileInfo.CreationTimeUtc,
            UpdatedAt = FileInfo.LastWriteTimeUtc,
            IsFolder = false,
            Owner = User
        };
    }

    public async Task<FileItem?> GetFileAsync(Guid fileId, Guid userId)
    {
        return null;
    }

    public async Task<IEnumerable<FileItem>> GetFilesAsync(Guid ownerId, string? parentPath = null)
    {
        return null;
    }

    public async Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        return null;
    }

    public async Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        return false;
    }

    public async Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId)
    {
        return null;
    }

    public async Task<FileItem?> RenameFileAsync(Guid fileId, string newName, Guid userId)
    {
        return null;
    }
}