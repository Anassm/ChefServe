using ChefServe.Core.Models;
using ChefServe.Core.DTOs;


namespace ChefServe.Core.Interfaces;
public interface IFileService
{
    Task<FileItem> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath);

    Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath);

    Task<FileItem?> GetFileAsync(Guid fileId, Guid userId);

    Task<IEnumerable<FileItem>> GetFilesAsync(Guid ownerId, string? parentPath = null);

    Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId);

    Task<bool> DeleteFileAsync(Guid fileId, Guid userId);

    Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId);

    Task<FileItem?> RenameFileAsync(Guid fileId, string newName, Guid userId);

    Task<getFileTreeReturnDTO> GetFileTreeAsync(Guid ownerId);

    Task<int> GetFileCountAsync();

    Task<int> GetFolderCountAsync();

    Task<int> GetFileTypeCountAsync();

    Task<List<(string, int)>> GetFileTypeStatisticsAsync();
}