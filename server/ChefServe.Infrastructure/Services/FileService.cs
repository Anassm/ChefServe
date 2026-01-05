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

                HasContentAsync(fileItem.ParentPath!, ownerId);

                var returnFileItem = new FileItemDTO
                {
                    ID = fileItem.ID,
                    Name = fileItem.Name,
                    Path = fileItem.Path,
                    Extension = fileItem.Extension ?? string.Empty,
                    IsFolder = fileItem.IsFolder,
                    OwnerID = fileItem.OwnerID,
                    CreatedAt = fileItem.CreatedAt,
                    UpdatedAt = fileItem.UpdatedAt
                };

                return new FileServiceResponseDTO
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Folder created successfully.",
                    Data = returnFileItem
                };
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                    if (Directory.Exists(fullPath))
                    {
                        Directory.Delete(fullPath, true);
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
            string dirPath = UserHelper.GetRootPathForUser(ownerId);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
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

            if (!Directory.Exists(dirPath))
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Destination path does not exist."
                };
            }

            fileName = FileHelper.SanitizeFileName(fileName);
            string dbPath = Path.Combine(dirPath, fileName);
            string fullPath = Path.GetFullPath(dbPath);

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
                            using (var fileStream = new FileStream(
                                fullPath,
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None,
                                81920,
                                useAsync: true))
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

                            using (var fileStream = new FileStream(
                                fullPath,
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None,
                                81920,
                                useAsync: true))
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

                            var returnFileItem = new FileItemDTO
                            {
                                ID = fileitem.ID,
                                Name = fileitem.Name,
                                Path = fileitem.Path,
                                Extension = fileitem.Extension,
                                IsFolder = fileitem.IsFolder,
                                OwnerID = fileitem.OwnerID,
                                CreatedAt = fileitem.CreatedAt,
                                UpdatedAt = fileitem.UpdatedAt
                            };

                            return new FileServiceResponseDTO
                            {
                                Success = true,
                                StatusCode = 201,
                                Message = "File uploaded successfully with suffix.",
                                Data = returnFileItem
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
                using (var fileStream = new FileStream(
                    fullPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    useAsync: true))
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

                var returnFileItem = new FileItemDTO
                {
                    ID = fileitem.ID,
                    Name = fileitem.Name,
                    Path = fileitem.Path,
                    Extension = fileitem.Extension,
                    IsFolder = fileitem.IsFolder,
                    OwnerID = fileitem.OwnerID,
                    CreatedAt = fileitem.CreatedAt,
                    UpdatedAt = fileitem.UpdatedAt
                };

                return new FileServiceResponseDTO
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "File uploaded successfully.",
                    Data = returnFileItem
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

            var returnFileItem = new FileItemDTO
            {
                ID = fileItem.ID,
                Name = fileItem.Name,
                Path = fileItem.Path,
                Extension = fileItem.Extension ?? string.Empty,
                IsFolder = fileItem.IsFolder,
                OwnerID = fileItem.OwnerID,
                CreatedAt = fileItem.CreatedAt,
                UpdatedAt = fileItem.UpdatedAt
            };

            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File retrieved successfully.",
                Data = returnFileItem
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
                parentPath = parentPath.Replace('/', '\\');
                parentPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), parentPath);
                files = await _context.FileItems.Where(f => f.OwnerID.ToString().ToUpper() == ownerId.ToString().ToUpper() &&
                    f.ParentPath.ToUpper() == parentPath.ToUpper()).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name.ToLower()).ToListAsync();
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

                    var childItems = _context.FileItems
                        .Where(f => f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar))
                        .ToList();

                    _context.FileItems.RemoveRange(childItems);
                    HasContentAsync(fileItem.ParentPath!, userId);
                }
            }
            else
            {
                if (File.Exists(fileItem.Path))
                {
                    File.Delete(fileItem.Path);
                    HasContentAsync(fileItem.ParentPath!, userId);
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

    public async Task<FileServiceResponseDTO> RenameFileAsync( Guid fileId, string newName, Guid userId)
    {
        var fileItem = await _context.FileItems
            .FirstOrDefaultAsync(f => f.ID == fileId && f.OwnerID == userId);

        if (fileItem == null)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 404,
                Message = "File not found."
            };
        }

        var extension = fileItem.IsFolder ? string.Empty : fileItem.Extension ?? Path.GetExtension(fileItem.Path);

        var newFullPath = Path.Combine(fileItem.ParentPath, fileItem.IsFolder ? newName : newName + extension);

        if (fileItem.IsFolder)
        {
            if (Directory.Exists(newFullPath) ||
                await _context.FileItems.AnyAsync(f =>
                    f.Path == newFullPath &&
                    f.OwnerID == userId))
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "A folder with the new name already exists."
                };
            }
        }
        else
        {
            if (File.Exists(newFullPath) ||
                await _context.FileItems.AnyAsync(f =>
                    f.Path == newFullPath &&
                    f.OwnerID == userId &&
                    !f.IsFolder))
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "A file with the new name already exists."
                };
            }
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var oldPath = fileItem.Path;

            if (fileItem.IsFolder)
            {
                Directory.Move(oldPath, newFullPath);

                var children = await _context.FileItems
                    .Where(f => f.Path.StartsWith(oldPath + Path.DirectorySeparatorChar))
                    .ToListAsync();

                foreach (var child in children)
                {
                    child.Path = child.Path.Replace(oldPath, newFullPath);
                    child.ParentPath = Path.GetDirectoryName(child.Path)!;
                }
            }
            else
            {
                File.Move(oldPath, newFullPath);
            }

            fileItem.Name = newName + extension;
            fileItem.Path = newFullPath;
            fileItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File renamed successfully.",
                Data = new FileItemDTO
                {
                    ID = fileItem.ID,
                    Name = fileItem.Name,
                    Path = fileItem.Path,
                    Extension = extension,
                    IsFolder = fileItem.IsFolder,
                    OwnerID = fileItem.OwnerID,
                    CreatedAt = fileItem.CreatedAt,
                    UpdatedAt = fileItem.UpdatedAt
                }
            };
        }
        catch (Exception ex)
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch
            {

            }

            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while renaming the file.",
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
                hasContent = fileItems.Any(item => item.ParentPath == f.Path && item.IsFolder),
                children = new List<GetFileTreeReturnDTO>()
            });

            bool userHasContent = lookup.Values.Any(item => item.parentPath == rootPath);

            var virtualRoot = new GetFileTreeReturnDTO
            {
                id = Guid.Empty,
                name = "root",
                folderPath = rootPath,
                parentPath = string.Empty,
                hasContent = fileItems.Any(item => item.ParentPath == rootPath && item.IsFolder),
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
            void UpdateDisplayPaths(GetFileTreeReturnDTO node)
            {
                if (!string.IsNullOrEmpty(node.folderPath) && node.folderPath.StartsWith(rootPath))
                    node.folderPath = node.folderPath.Substring(rootPath.Length).TrimStart('/', '\\');

                if (!string.IsNullOrEmpty(node.parentPath) && node.parentPath.StartsWith(rootPath))
                    node.parentPath = node.parentPath.Substring(rootPath.Length).TrimStart('/', '\\');

                node.children = node.children.OrderBy(c => c.name).ToList();

                foreach (var child in node.children)
                    UpdateDisplayPaths(child);
            }
            UpdateDisplayPaths(virtualRoot);

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
                hasContent = false,
                children = new List<GetFileTreeReturnDTO>()
            };
        }
    }

    public async Task<FileServiceResponseDTO> GetFileInfoAsync(Guid userId, Guid fileId)
    {
        try
        {
            Console.WriteLine("testahaha: " + fileId);

            var fileData = await _context.FileItems
                .Where(f => f.OwnerID == userId && f.ID == fileId)
                .FirstOrDefaultAsync();

            if (fileData == null)
            {
                return new FileServiceResponseDTO
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "File not found."
                };
            }

            FileInfo fileinfo = new FileInfo(fileData.Path);
            return new FileServiceResponseDTO
            {
                Success = true,
                StatusCode = 200,
                Message = "File info retrieved successfully.",
                Data = new
                {
                    fileData.Name,
                    fileData.Path,
                    fileData.ParentPath,
                    fileData.Extension,
                    fileData.IsFolder,
                    fileData.CreatedAt,
                    fileData.UpdatedAt,
                    SizeInBytes = fileData.IsFolder ? 0 : fileinfo.Length,
                    SizeInMB = fileData.IsFolder ? 0 : fileinfo.Length / (1024.0 * 1024.0),
                }
            };
        }
        catch (Exception ex)
        {
            return new FileServiceResponseDTO
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred while fetching file data: " + ex.Message
            };
        }
    }

    public async Task<int> GetFileCountAsync()
    {
        return await _context.FileItems.CountAsync(f => !f.IsFolder);
    }

    public async Task<int> GetFolderCountAsync()
    {
        return await _context.FileItems.CountAsync(f => f.IsFolder);
    }

    public async Task<int> GetFileTypeCountAsync()
    {
        return await _context.FileItems
            .Where(f => !f.IsFolder)
            .Select(f => f.Extension)
            .Distinct()
            .CountAsync();
    }

    public async Task<List<(string, int)>> GetFileTypeStatisticsAsync()
    {
        var stats = await _context.FileItems
            .Where(f => !f.IsFolder)
            .GroupBy(f => f.Extension)
            .Select(g => new { Extension = g.Key ?? "No Extension", Count = g.Count() })
            .ToListAsync();

        return stats.Select(s => (s.Extension, s.Count)).ToList();
    }
}