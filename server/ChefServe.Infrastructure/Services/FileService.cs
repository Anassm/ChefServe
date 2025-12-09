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


    public async Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
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
                    usedNumbers.Add(0); // base name exists
                else
                {
                    var match = System.Text.RegularExpressions.Regex.Match(
                        name, $@"^{Regex.Escape(folderName)} \((\d+)\)$"
                    );
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
                    break; // gap found
            }

            var newFolderName = suffix == 0 ? folderName : $"{folderName} ({suffix})";
            dbPath = Path.Combine(parentDir, newFolderName);
            fullPath = Path.GetFullPath(dbPath);
        }

        Directory.CreateDirectory(fullPath);

        var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
        if (User == null)
            return null;

        var dirInfo = new DirectoryInfo(fullPath);
        var fileitem = new FileItem
        {
            Name = dirInfo.Name,
            Path = dbPath,
            ParentPath = Path.GetDirectoryName(dbPath),
            Extension = null,
            IsFolder = true,
            OwnerID = ownerId,
            Owner = User
        };

        _context.FileItems.Add(fileitem);
        await _context.SaveChangesAsync();
        return fileitem;
    }

    public async Task<FileItem> UploadFileAsync(Guid ownerId, string fileName, Stream content, string? destinationPath)
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
            return null!;
        }


        fileName = FileHelper.SanitizeFileName(fileName);
        var dbPath = Path.Combine(dirPath, fileName);
        var fullPath = Path.GetFullPath(dbPath);
        if (File.Exists(fullPath))
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

            // Find the lowest unused number
            int suffix = 0;
            usedNumbers.Sort();
            foreach (var n in usedNumbers)
            {
                if (n == suffix)
                    suffix++;
                else
                    break; // found a gap
            }

            // Determine new file name
            string newFileName = suffix == 0 ? fileName : $"{baseName} ({suffix}){extension}";
            dbPath = Path.Combine(dirPath, newFileName);
            fullPath = Path.GetFullPath(dbPath);
        }

        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream);
        }

        var FileInfo = new FileInfo(fullPath);
        var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
        if (User == null)
            return null;

        var fileitem = new FileItem
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
        return fileitem;
    }

    public async Task<FileItem?> GetFileAsync(Guid fileId, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty)
            return null;

        var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();

        if (fileItem == null)
            return null;

        return fileItem;
    }

    public async Task<IEnumerable<FileItem>> GetFilesAsync(Guid ownerId, string? parentPath = null)
    {
        if (parentPath == null || parentPath.Trim() == string.Empty)
        {
            parentPath = UserHelper.GetRootPathForUser(ownerId);
            return await _context.FileItems.Where(f => f.OwnerID.ToString().ToUpper() == ownerId.ToString().ToUpper() &&
            f.ParentPath.ToUpper() == parentPath.ToUpper()).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
        }
        parentPath = parentPath.TrimStart('/', '\\');
        parentPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), parentPath);
        return await _context.FileItems.Where(f => f.OwnerID.ToString().ToUpper() == ownerId.ToString().ToUpper() &&
            f.ParentPath.ToUpper() == parentPath.ToUpper()).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
    }

    public async Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();
        if (fileItem == null || fileItem.IsFolder || !File.Exists(fileItem.Path))
            return null;

        return new FileStream(fileItem.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
        if (fileItem == null)
            return false;

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
        return true;
    }

    public async Task<FileItem?> MoveFileAsync(Guid fileId, string newPath, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty || newPath == null || newPath.Trim() == string.Empty)
            return null;

        var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
        if (fileItem == null)
            return null;

        newPath = newPath.Trim().TrimEnd('/', '\\').TrimStart('/', '\\');
        if (newPath == null || newPath == string.Empty)
            newPath = UserHelper.GetRootPathForUser(userId);
        else
            newPath = Path.Combine(UserHelper.GetRootPathForUser(userId), newPath);

        string oldParentPath = fileItem.ParentPath == null ? string.Empty : fileItem.ParentPath.TrimEnd('/', '\\');
        var destinationFullPath = Path.Combine(newPath, fileItem.Name);
        if (fileItem.IsFolder)
        {
            if (Directory.Exists(destinationFullPath))
            {
                return null;
            }
            Directory.Move(fileItem.Path, destinationFullPath);
            if (!Directory.Exists(destinationFullPath))
            {
                return null;
            }
            var movedItems = await _context.FileItems.Where(f => f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar)).ToListAsync();
            foreach (var item in movedItems)
            {
                item.Path = item.Path.Replace(fileItem.Path, destinationFullPath);
                item.ParentPath = Path.GetDirectoryName(item.Path);
            }
            fileItem.Path = destinationFullPath;
            fileItem.ParentPath = Path.GetDirectoryName(destinationFullPath);
        }
        else
        {
            if (File.Exists(destinationFullPath))
            {
                return null;
            }
            File.Move(fileItem.Path, destinationFullPath);

            fileItem.Path = destinationFullPath;
        }
        await _context.SaveChangesAsync();
        HasContentAsync(oldParentPath, userId);
        HasContentAsync(fileItem.ParentPath!, userId);
        return fileItem;
    }

    public async Task<FileItem?> RenameFileAsync(Guid fileId, string newName, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty || newName == null || newName.Trim() == string.Empty)
            return null;

        var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();
        if (fileItem == null)
            return null;

        var newFullPath = fileItem.ParentPath + Path.DirectorySeparatorChar + newName;
        if (fileItem.IsFolder)
        {
            if (Directory.Exists(newFullPath))
            {
                return null;
            }
            Directory.Move(fileItem.Path, newFullPath);
            if (!Directory.Exists(newFullPath))
            {
                return null;
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
                return null;
            }
            File.Move(fileItem.Path, newFullPath);

            fileItem.Path = newFullPath;
            fileItem.Name = newName;
        }

        await _context.SaveChangesAsync();
        return fileItem;
    }

    public async Task<getFileTreeReturnDTO> GetFileTreeAsync(Guid userId)
    {
        var rootPath = UserHelper.GetRootPathForUser(userId);
        var fileItems = await _context.FileItems
            .Where(f => f.OwnerID == userId && f.IsFolder)
            .ToListAsync();

        var lookup = fileItems.ToDictionary(f => f.ID, f => new getFileTreeReturnDTO
        {
            id = f.ID,
            name = f.Name,
            folderPath = f.Path,
            parentPath = f.ParentPath,
            children = new List<getFileTreeReturnDTO>()
        });

        var virtualRoot = new getFileTreeReturnDTO
        {
            id = Guid.Empty,
            name = "root",
            folderPath = rootPath,
            parentPath = string.Empty,
            children = new List<getFileTreeReturnDTO>()
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