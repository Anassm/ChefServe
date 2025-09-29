using ChefServe.Core.Models;

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
}
