using ChefServe.Core.Models;
using ChefServe.Core.DTOs;


namespace ChefServe.Core.Interfaces;
public interface IFileService
{
    //Task<UploadFileDTO> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath);

    Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath);

    Task<FileItem?> GetFileAsync(Guid fileId, Guid userId);

    Task<IEnumerable<FileItem>> GetFilesAsync(Guid ownerId, string? parentPath = null);

    Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId);

    Task<bool> DeleteFileAsync(Guid fileId, Guid userId);

    Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId);

    Task<FileItem?> RenameFileAsync(Guid fileId, string newName, Guid userId);
}