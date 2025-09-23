using ChefServe.Core.Models;

public interface IFileService
{
    // Upload een bestand naar een folder
    Task<FileItem> UploadFileAsync(
        Guid ownerId,
        string fileName,
        Stream content,
        string destinationPath);

    // Maak een folder aan
    Task<FileItem> CreateFolderAsync(
        Guid ownerId,
        string folderName,
        string parentPath);

    // Haal 1 bestand/folder op
    Task<FileItem?> GetFileAsync(Guid fileId, Guid userId);

    // Haal alle bestanden/folders van een user of folder op
    Task<IEnumerable<FileItem>> GetFilesAsync(
        Guid ownerId,
        string? parentPath = null);

    // Download een bestand als stream
    Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId);

    // Verwijder een bestand/folder
    Task<bool> DeleteFileAsync(Guid fileId, Guid userId);

    // Verplaats een bestand/folder naar een andere map
    Task<FileItem?> MoveFileAsync(
        Guid fileId,
        string newPath,
        Guid userId);

    // Hernoem een bestand/folder
    Task<FileItem?> RenameFileAsync(
        Guid fileId,
        string newName,
        Guid userId);
}
