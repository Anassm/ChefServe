using ChefServe.Core.Models;
public interface IFileService
{
    Task<FileItem> UploadFileAsync(string fileName, Stream fileStream, string contentType, string userId, int? parentFolderId = null);
    Task<FileItem> CreateFolderAsync(string folderName, string userId, int? parentFolderId = null);
    Task<FileItem?> GetFileAsync(int fileId, string userId);
    Task<IEnumerable<FileItem>> GetFilesAsync(string userId, int? parentFolderId = null);
    Task<Stream?> DownloadFileAsync(int fileId, string userId);
    Task<bool> DeleteFileAsync(int fileId, string userId);
    Task<FileItem?> MoveFileAsync(int fileId, int? newParentFolderId, string userId);
    Task<FileItem?> RenameFileAsync(int fileId, string newName, string userId);
}