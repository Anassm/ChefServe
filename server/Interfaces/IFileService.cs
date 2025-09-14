public interface IFileService
{
    Task<File> UploadFileAsync(string fileName, Stream fileStream, string contentType, string userId, int? parentFolderId = null);
    Task<File> CreateFolderAsync(string folderName, string userId, int? parentFolderId = null);
    Task<File?> GetFileAsync(int fileId, string userId);
    Task<IEnumerable<File>> GetFilesAsync(string userId, int? parentFolderId = null);
    Task<Stream?> DownloadFileAsync(int fileId, string userId);
    Task<bool> DeleteFileAsync(int fileId, string userId);
    Task<File?> MoveFileAsync(int fileId, int? newParentFolderId, string userId);
    Task<File?> RenameFileAsync(int fileId, string newName, string userId);
}