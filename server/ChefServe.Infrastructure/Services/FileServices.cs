using ChefServe.Core.DTOs;
using ChefServe.Core.Models;
using ChefServe.Core.Interfaces;

namespace ChefServe.Infrastructure.Services;

public class FileServices : IFileService
{
    public Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
        return null;
    }

    public Task<FileItem> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath)
    {
        return null;
    }

    public Task<FileItem?> GetFileAsync(Guid fileId, Guid userId)
    {
        return null;
    }

    public Task<IEnumerable<FileItem>> GetFilesAsync(Guid ownerId, string? parentPath = null)
    {
        return null;
    }

    public Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        return null;
    }

    public Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        return null;
    }

    public Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId)
    {
        return null;
    }

    public Task<FileItem?> RenameFileAsync(Guid fileId, string newName, Guid userId)
    {
        return null;
    }
}