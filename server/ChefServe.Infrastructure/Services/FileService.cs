using ChefServe.Core.DTOs;
using ChefServe.Core.Models;
using ChefServe.Core.Helper;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace ChefServe.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly ChefServeDbContext _context;
    public FileService(ChefServeDbContext context)
    {
        _context = context;
    }

    private async void HasContentAsync(string folderPath, Guid ownerId)
    {
        if (ownerId == Guid.Empty)
            return;

        if (string.IsNullOrWhiteSpace(folderPath))
            folderPath = UserHelper.GetRootPathForUser(ownerId);

        var has = await _context.FileItems
            .AnyAsync(f => f.OwnerID == ownerId && f.ParentPath == folderPath);

        if (has)
        {
            var folderItem = await _context.FileItems
                .Where(f => f.OwnerID == ownerId && f.Path == folderPath && f.IsFolder)
                .FirstOrDefaultAsync();
            if (folderItem != null)
            {
                folderItem.HasContent = true;
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            var folderItem = await _context.FileItems
                .Where(f => f.OwnerID == ownerId && f.Path == folderPath && f.IsFolder)
                .FirstOrDefaultAsync();
            if (folderItem != null)
            {
                folderItem.HasContent = false;
                await _context.SaveChangesAsync();
            }
        }
    }


    public async Task<FileServiceResponseDTO> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
        try
        {
            var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
            if (User == null)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found."
                };

            string fullPath = string.Empty;
            string dbPath = string.Empty;

            var cleanParentPath = parentPath.TrimStart('/', '\\').Replace('/', '\\');
            if (cleanParentPath == null || cleanParentPath.Trim() == string.Empty)
            {
                dbPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), folderName);
                fullPath = Path.GetFullPath(dbPath);
            }
            else
            {
                dbPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), cleanParentPath, folderName);
                fullPath = Path.GetFullPath(dbPath);
            }
            if (Directory.Exists(fullPath))
            {
                var parentDir = Path.GetDirectoryName(dbPath) ?? UserHelper.GetRootPathForUser(ownerId);

                var existingFolders = await _context.FileItems
                .Where(f => f.ParentPath == parentDir && f.Name.StartsWith(folderName))
                .Select(f => f.Name)
                .ToListAsync();

                var usedNumbers = new List<int>();
                foreach (var name in existingFolders)
                {
                    if (name == folderName)
                        usedNumbers.Add(0);
                    else
                    {
                        var match = Regex.Match(
                            name, $@"^{Regex.Escape(folderName)} \((\d+)\)$"
                        );
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int n))
                            usedNumbers.Add(n);
                    }
                }

                int number = 0;
                usedNumbers.Sort();
                foreach (var num in usedNumbers)
                {
                    if (num == number)
                        number++;
                    else
                        break;
                }

                var newFolderName = number == 0 ? folderName : $"{folderName} ({number})";
                dbPath = Path.Combine(parentDir, newFolderName);
                fullPath = Path.GetFullPath(dbPath);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Directory.CreateDirectory(fullPath);

                var dirInfo = new DirectoryInfo(fullPath);
                var fileItem = new FileItem
                {
                    Name = dirInfo.Name,
                    Path = dbPath,
                    ParentPath = Path.GetDirectoryName(dbPath),
                    Extension = null,
                    IsFolder = true,
                    OwnerID = ownerId,
                    Owner = User
                };

                _context.FileItems.Add(fileItem);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new FileServiceResponseDTO
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Folder created successfully.",
                    Data = fileItem
                };
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                    if (Directory.Exists(fullPath))
                    {
                        Directory.Delete(fullPath, false);
                    }
                }
                catch (Exception rollbackEx)
                {
                    System.Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to create directory: " + ex.Message
                };
            }
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while creating the folder: " + ex.Message
            };
        }
    }

    public async Task<FileServiceResponseDTO> UploadFileAsync(Guid ownerId, string fileName, Stream content, string? destinationPath, FileConflictMode? conflictMode)
    {
        try
        {

            // Ensure user directory exists
            string dirPath = UserHelper.GetRootPathForUser(ownerId);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            // Process destination path
            if (destinationPath == null)
            {
                destinationPath = string.Empty;
            }
            else
            {
                destinationPath = destinationPath.Trim('/', '\\');
            }
            if (destinationPath.Trim() == string.Empty)
            {
                destinationPath = string.Empty;
            }
            destinationPath = destinationPath.Replace("/", "\\");
            dirPath = Path.Combine(dirPath, destinationPath);
            // Check if destination path exists
            if (!Directory.Exists(dirPath))
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Destination path does not exist."
                };
            }
            // Sanitize file name and prepare full path
            fileName = FileHelper.SanitizeFileName(fileName);
            string dbPath = Path.Combine(dirPath, fileName);
            string fullPath = Path.GetFullPath(dbPath);
            // Handle file name conflicts
            if (File.Exists(fullPath))
            {
                switch (conflictMode)
                {
                    case FileConflictMode.Overwrite:
                        var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            File.Delete(fullPath);
                            var existingFileItem = await _context.FileItems
                                .Where(f => f.Path == dbPath && f.OwnerID == ownerId)
                                .FirstOrDefaultAsync();
                            if (existingFileItem != null)
                            {
                                existingFileItem.UpdatedAt = DateTime.UtcNow;
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                            }
                            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                            {
                                await content.CopyToAsync(fileStream);
                            }
                            return new FileServiceResponseDTO
                            {
                                Success = true,
                                StatusCode = 201,
                                Message = "File overwritten successfully.",
                                Data = existingFileItem
                            };
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                await transaction.RollbackAsync();
                            }
                            catch (Exception rollbackEx)
                            {
                                System.Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                            }
                            return new FileServiceResponseDTO
                            {
                                Success = false,
                                StatusCode = 500,
                                Message = "Failed to overwrite existing file: " + ex.Message
                            };
                        }
                    case FileConflictMode.Suffix:
                        var transaction2 = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            var existingFiles = await _context.FileItems
                            .Where(f => f.ParentPath == dirPath && f.Name.StartsWith(Path.GetFileNameWithoutExtension(fileName)) && !f.IsFolder)
                            .Select(f => f.Name)
                            .ToListAsync();

                            var usedNumbers = new List<int>();
                            string baseName = Path.GetFileNameWithoutExtension(fileName);
                            string extension = Path.GetExtension(fileName);

                            foreach (var name in existingFiles)
                            {
                                if (name == baseName + extension)
                                    usedNumbers.Add(0);
                                else
                                {
                                    var match = Regex.Match(name, $@"^{Regex.Escape(baseName)} \((\d+)\){Regex.Escape(extension)}$");
                                    if (match.Success && int.TryParse(match.Groups[1].Value, out int n))
                                        usedNumbers.Add(n);
                                }
                            }

                            int suffix = 0;
                            usedNumbers.Sort();
                            foreach (var n in usedNumbers)
                            {
                                if (n == suffix)
                                    suffix++;
                                else
                                    break;
                            }

                            string newFileName = suffix == 0 ? fileName : $"{baseName} ({suffix}){extension}";
                            dbPath = Path.Combine(dirPath, newFileName);
                            fullPath = Path.GetFullPath(dbPath);

                            // Save file record with new name
                            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                            {
                                await content.CopyToAsync(fileStream);
                            }
                            var FileInfo = new FileInfo(fullPath);
                            var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
                            if (User == null)
                                return new FileServiceResponseDTO
                                {
                                    Success = false,
                                    StatusCode = 404,
                                    Message = "User not found."
                                };

                            FileItem fileitem = new FileItem
                            {
                                Name = FileInfo.Name,
                                Path = dbPath,
                                ParentPath = Path.GetDirectoryName(dbPath),
                                Extension = FileInfo.Extension,
                                OwnerID = ownerId,
                                CreatedAt = FileInfo.CreationTimeUtc,
                                UpdatedAt = FileInfo.LastWriteTimeUtc,
                                IsFolder = false,
                                Owner = User
                            };
                            _context.FileItems.Add(fileitem);
                            await _context.SaveChangesAsync();
                            await transaction2.CommitAsync();
                            HasContentAsync(fileitem.ParentPath, ownerId);
                            return new FileServiceResponseDTO
                            {
                                Success = true,
                                StatusCode = 201,
                                Message = "File uploaded successfully with suffix.",
                                Data = fileitem
                            };
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                await transaction2.RollbackAsync();
                                if (File.Exists(fullPath))
                                {
                                    File.Delete(fullPath);
                                }
                            }
                            catch (Exception rollbackEx)
                            {
                                System.Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                            }
                            return new FileServiceResponseDTO
                            {
                                Success = false,
                                StatusCode = 500,
                                Message = "Failed to upload file with suffix: " + ex.Message
                            };
                        }
                    case FileConflictMode.Cancel:
                        return new FileServiceResponseDTO
                        {
                            Success = true,
                            StatusCode = 204,
                            Message = "Upload canceled."
                        };
                    case null:
                        return new FileServiceResponseDTO
                        {
                            Success = false,
                            StatusCode = 409,
                            Message = "File already exists, no conflict mode specified."
                        };
                    default:
                        return new FileServiceResponseDTO
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = $"Invalid conflict mode: '{conflictMode}' not in [overwrite, suffix, cancel]."
                        };
                }
            }
            else
            {
                using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    await content.CopyToAsync(fileStream);
                }

                var FileInfo = new FileInfo(fullPath);
                var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
                if (User == null)
                    return new FileServiceResponseDTO
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "User not found."
                    };

                FileItem fileitem = new FileItem
                {
                    Name = FileInfo.Name,
                    Path = dbPath,
                    ParentPath = Path.GetDirectoryName(dbPath),
                    Extension = FileInfo.Extension,
                    OwnerID = ownerId,
                    CreatedAt = FileInfo.CreationTimeUtc,
                    UpdatedAt = FileInfo.LastWriteTimeUtc,
                    IsFolder = false,
                    Owner = User
                };
                _context.FileItems.Add(fileitem);
                await _context.SaveChangesAsync();
                HasContentAsync(fileitem.ParentPath, ownerId);
                return new FileServiceResponseDTO
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "File uploaded successfully.",
                    Data = fileitem
                };
            }
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred during file upload: " + ex.Message
            };
        }
    }

    public async Task<FileServiceResponseDTO> GetFileAsync(Guid fileId, Guid userId)
    {
        try
        {
            if (fileId == Guid.Empty || userId == Guid.Empty)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Invalid file ID or user ID.",
                    Data = null
                };

            var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();

            if (fileItem == null)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "File not found.",
                    Data = null
                };

            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File retrieved successfully.",
                Data = fileItem
            };
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while retrieving the file: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<FileServiceResponseDTO> GetFilesAsync(Guid ownerId, string? parentPath = null)
    {
        try
        {
            IEnumerable<FileItem> files;
            if (parentPath == null || parentPath.Trim() == string.Empty)
            {
                parentPath = UserHelper.GetRootPathForUser(ownerId);
                files = await _context.FileItems.Where(f => f.OwnerID.ToString().ToUpper() == ownerId.ToString().ToUpper() &&
                f.ParentPath.ToUpper() == parentPath.ToUpper()).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
            }
            else
            {
                parentPath = parentPath.TrimStart('/', '\\');
                parentPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), parentPath);
                files = await _context.FileItems.Where(f => f.OwnerID.ToString().ToUpper() == ownerId.ToString().ToUpper() &&
                    f.ParentPath.ToUpper() == parentPath.ToUpper()).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
            }
            if (files == null || !files.Any())
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 204,
                    Message = "No files found."
                };
            }
            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "Files retrieved successfully.",
                Data = files
            };
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while retrieving files." + ex.Message
            };
        }
    }

    public async Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        try
        {
            var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();
            if (fileItem == null || fileItem.IsFolder || !File.Exists(fileItem.Path))
                return null;

            return new FileStream(fileItem.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch
        {
            return null;
        }
    }

    public async Task<FileServiceResponseDTO> DeleteFileAsync(Guid fileId, Guid userId)
    {
        try
        {
            var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
            if (fileItem == null)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "File not found."
                };

            if (fileItem.IsFolder)
            {
                if (Directory.Exists(fileItem.Path))
                {
                    Directory.Delete(fileItem.Path, true);
                    HasContentAsync(fileItem.ParentPath!, userId);
                }
            }
            else
            {
                if (File.Exists(fileItem.Path))
                {
                    File.Delete(fileItem.Path);
                }
            }

            _context.FileItems.Remove(fileItem);
            await _context.SaveChangesAsync();
            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while deleting the file: " + ex.Message
            };
        }
    }

    // public async Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId)
    // {
    //     if (fileId == Guid.Empty || userId == Guid.Empty || newPath == null || newPath.Trim() == string.Empty)
    //         return null;

    //     var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
    //     if (fileItem == null)
    //         return null;

    //     newPath = newPath.Trim().TrimEnd('/', '\\').TrimStart('/', '\\');
    //     if (newPath == null || newPath == string.Empty)
    //         newPath = UserHelper.GetRootPathForUser(userId);
    //     else
    //         newPath = Path.Combine(UserHelper.GetRootPathForUser(userId), newPath);

    //     string oldParentPath = fileItem.ParentPath == null ? string.Empty : fileItem.ParentPath.TrimEnd('/', '\\');
    //     var destinationFullPath = Path.Combine(newPath, fileItem.Name);
    //     if (fileItem.IsFolder)
    //     {
    //         if (Directory.Exists(destinationFullPath))
    //         {
    //             return null;
    //         }
    //         Directory.Move(fileItem.Path, destinationFullPath);
    //         if (!Directory.Exists(destinationFullPath))
    //         {
    //             return null;
    //         }
    //         var movedItems = await _context.FileItems.Where(f => f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar)).ToListAsync();
    //         foreach (var item in movedItems)
    //         {
    //             item.Path = item.Path.Replace(fileItem.Path, destinationFullPath);
    //             item.ParentPath = Path.GetDirectoryName(item.Path);
    //         }
    //         fileItem.Path = destinationFullPath;
    //         fileItem.ParentPath = Path.GetDirectoryName(destinationFullPath);
    //     }
    //     else
    //     {
    //         if (File.Exists(destinationFullPath))
    //         {
    //             return null;
    //         }
    //         File.Move(fileItem.Path, destinationFullPath);

    //         fileItem.Path = destinationFullPath;
    //     }
    //     await _context.SaveChangesAsync();
    //     HasContentAsync(oldParentPath, userId);
    //     HasContentAsync(fileItem.ParentPath!, userId);
    //     return fileItem;
    // }

    public async Task<FileServiceResponseDTO> RenameFileAsync(Guid fileId, string newName, Guid userId)
    {
        try
        {

            if (fileId == Guid.Empty || userId == Guid.Empty || newName == null || newName.Trim() == string.Empty)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Invalid file ID, user ID, or new name."
                };

            var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();
            if (fileItem == null)
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "File not found."
                };

            var newFullPath = fileItem.ParentPath + Path.DirectorySeparatorChar + newName;
            if (fileItem.IsFolder)
            {
                if (Directory.Exists(newFullPath))
                {
                    return new FileServiceResponseDTO
                    {
                        Success = false,
                        StatusCode = 409,
                        Message = "A folder with the new name already exists."
                    };
                }
                Directory.Move(fileItem.Path, newFullPath);
                if (!Directory.Exists(newFullPath))
                {
                    return new FileServiceResponseDTO
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = "Failed to rename folder."
                    };
                }
                var renamedItems = await _context.FileItems.Where(f => f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar)).ToListAsync();
                foreach (var item in renamedItems)
                {
                    item.Path = item.Path.Replace(fileItem.Path, newFullPath);
                    item.ParentPath = Path.GetDirectoryName(item.Path);
                }
                fileItem.Name = newName;
                fileItem.Path = newFullPath;
            }
            else
            {
                if (File.Exists(newFullPath))
                {
                    return new FileServiceResponseDTO
                    {
                        Success = false,
                        StatusCode = 409,
                        Message = "A file with the new name already exists."
                    };
                }
                File.Move(fileItem.Path, newFullPath);

                fileItem.Path = newFullPath;
                fileItem.Name = newName;
            }

            await _context.SaveChangesAsync();
            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File renamed successfully.",
                Data = fileItem
            };
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while renaming the file: " + ex.Message
            };
        }
    }

    public async Task<GetFileTreeReturnDTO> GetFileTreeAsync(Guid userId)
    {
        try
        {
            var rootPath = UserHelper.GetRootPathForUser(userId);
            var fileItems = await _context.FileItems
                .Where(f => f.OwnerID == userId && f.IsFolder)
                .ToListAsync();

            var lookup = fileItems.ToDictionary(f => f.ID, f => new GetFileTreeReturnDTO
            {
                id = f.ID,
                name = f.Name,
                folderPath = f.Path,
                parentPath = f.ParentPath,
                children = new List<GetFileTreeReturnDTO>()
            });

            var virtualRoot = new GetFileTreeReturnDTO
            {
                id = Guid.Empty,
                name = "root",
                folderPath = rootPath,
                parentPath = string.Empty,
                children = new List<GetFileTreeReturnDTO>()
            };

            foreach (var item in lookup.Values)
            {
                var parentPath = item.parentPath;

                if (parentPath == rootPath)
                {
                    virtualRoot.children.Add(item);
                }
                else if (parentPath != null)
                {
                    var parentItem = lookup.Values.FirstOrDefault(x => x.folderPath == parentPath);
                    if (parentItem != null)
                    {
                        parentItem.children.Add(item);
                    }
                    else
                    {
                        var toBeRemoved = _context.FileItems.Where(f => f.OwnerID == userId && f.ID == item.id).FirstOrDefault();
                        if (toBeRemoved != null)
                            _context.FileItems.Remove(toBeRemoved);
                    }
                }
            }

            return virtualRoot;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Error in GetFileTreeAsync: " + ex.Message);
            return new GetFileTreeReturnDTO
            {
                id = Guid.Empty,
                name = "root",
                folderPath = UserHelper.GetRootPathForUser(userId),
                parentPath = string.Empty,
                children = new List<GetFileTreeReturnDTO>()
            };
        }
    }
}