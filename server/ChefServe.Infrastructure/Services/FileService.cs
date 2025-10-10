using ChefServe.Core.DTOs;
using ChefServe.Core.Models;
using ChefServe.Core.Helper;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace ChefServe.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly ChefServeDbContext _context;
    public FileService(ChefServeDbContext context)
    {
        _context = context;
    }

    public async Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
        if (ownerId == Guid.Empty)
            return null;
        if (folderName == null || folderName.Trim() == string.Empty)
            return null;
        var fullPath = string.Empty;
        var cleanParentPath = parentPath.TrimStart('/', '\\');
        if (cleanParentPath == null || cleanParentPath.Trim() == string.Empty)
        {
            fullPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), folderName);
        }
        else
        {
            fullPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), cleanParentPath, folderName);
        }

        System.Console.WriteLine(fullPath);
        if (Directory.Exists(fullPath))
        {
            return null;
        }

        Directory.CreateDirectory(fullPath);
        var dirInfo = new DirectoryInfo(fullPath);
        var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
        if (User == null)
            return null;
        var fileitem = new FileItem
        {
            Name = dirInfo.Name,
            Path = fullPath,
            ParentPath = dirInfo.Parent?.FullName,
            Type = null,
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
        //checks
        if (ownerId == Guid.Empty)
            return null;
        if (fileName == null || fileName.Trim() == string.Empty)
            return null;
        if (content == null || content.Length == 0)
            return null;
        string dirPath = UserHelper.GetRootPathForUser(ownerId);
        if (destinationPath == null || destinationPath.Trim() == string.Empty || destinationPath.TrimEnd('/', '\\').TrimStart('/', '\\') == string.Empty)
            destinationPath = string.Empty;

        //create directory
        dirPath = Path.Combine(dirPath, destinationPath);
        if (!Directory.Exists(dirPath))
        {
            return null;
        }
        if (File.Exists(dirPath))
        {
            return null;
        }
        var fullPath = Path.Combine(dirPath, fileName);
        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream);
        }

        //get file info
        var FileInfo = new FileInfo(fullPath);
        var User = _context.Users.Where(u => u.ID == ownerId).FirstOrDefault();
        if (User == null)
            return null;

        var fileitem = new FileItem
        {
            Name = FileInfo.Name,
            Path = fullPath,
            ParentPath = dirPath,
            Type = FileInfo.Extension,
            OwnerID = ownerId,
            CreatedAt = FileInfo.CreationTimeUtc,
            UpdatedAt = FileInfo.LastWriteTimeUtc,
            IsFolder = false,
            Owner = User
        };

        _context.FileItems.Add(fileitem);
        await _context.SaveChangesAsync();
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
        if (ownerId == Guid.Empty)
            return Enumerable.Empty<FileItem>();

        if (parentPath == null || parentPath.Trim() == string.Empty)
        {
            parentPath = UserHelper.GetRootPathForUser(ownerId);
            return await _context.FileItems.Where(f => f.OwnerID == ownerId &&
            f.ParentPath == parentPath).OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
        }

        return await _context.FileItems.Where(f => f.OwnerID == ownerId && f.ParentPath == parentPath)
            .OrderByDescending(f => f.IsFolder).ThenBy(f => f.Name).ToListAsync();
    }

    public async Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty)
            return null;

        var fileItem = await _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefaultAsync();
        if (fileItem == null || fileItem.IsFolder || !File.Exists(fileItem.Path))
            return null;

        return new FileStream(fileItem.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty)
            return false;

        var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
        if (fileItem == null)
            return false;

        if (fileItem.IsFolder)
        {
            if (Directory.Exists(fileItem.Path))
            {
                Directory.Delete(fileItem.Path, true);
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
}