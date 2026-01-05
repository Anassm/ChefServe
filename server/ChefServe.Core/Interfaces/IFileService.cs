using ChefServe.Core.Models;
using ChefServe.Core.DTOs;


namespace ChefServe.Core.Interfaces;

public interface IFileService
{
    Task<FileServiceResponseDTO> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath, FileConflictMode? conflictMode);

    Task<FileServiceResponseDTO> CreateFolderAsync(Guid ownerId, string folderName, string parentPath);

    Task<FileServiceResponseDTO> GetFileInfoAsync(Guid fileId, Guid userId);

    Task<FileServiceResponseDTO> GetFileAsync(Guid fileId, Guid userId);

    Task<FileServiceResponseDTO> GetFilesAsync(Guid ownerId, string? parentPath = null);

    Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId);

    Task<FileServiceResponseDTO> DeleteFileAsync(Guid fileId, Guid userId);

    // Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId);

    Task<FileServiceResponseDTO> RenameFileAsync(Guid fileId, string newName, Guid userId);
    Task<GetFileTreeReturnDTO> GetFileTreeAsync(Guid ownerId);

    Task<int> GetFileCountAsync();

    Task<int> GetFolderCountAsync();

    Task<int> GetFileTypeCountAsync();

    Task<List<(string, int)>> GetFileTypeStatisticsAsync();

    Task<int> GetFoldersWithContentCountAsync();

    Task<int> GetEmptyFolderCountAsync();

    Task<decimal> GetTotalStorageUsedAsync();

    Task<decimal> GetUserStorageUsedAsync(Guid userId);

    Task<int> GetUserFileCountAsync(Guid userId);
}