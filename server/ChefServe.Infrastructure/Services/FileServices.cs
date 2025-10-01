using ChefServe.Core.DTOs;
using ChefServe.Core.Models;
using ChefServe.Core.Helper;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


public class FileServices : IFileService
{
    private readonly ChefServeDbContext _context;
    public FileServices(ChefServeDbContext context)
    {
        _context = context;
    }

    public async Task<FileItem> CreateFolderAsync(Guid ownerId, string folderName, string parentPath)
    {
        if (ownerId == Guid.Empty)
            return null;
        if (folderName == null || folderName.Trim() == string.Empty)
            return null;
        if (parentPath == null || parentPath.Trim() == string.Empty)
            return null;
        var fullPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), parentPath, folderName);
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
            IsFolder = true,
            OwnerID = ownerId,
            Owner = User
        };
        _context.FileItems.Add(fileitem);
        await _context.SaveChangesAsync();
        return fileitem;
    }

    public async Task<FileItem> UploadFileAsync(Guid ownerId, string fileName, Stream content, string destinationPath)
    {
        //checks
        if (ownerId == Guid.Empty)
            return null;
        if (fileName == null || fileName.Trim() == string.Empty)
            return null;
        if (content == null || content.Length == 0)
            return null;
        if (destinationPath == null || destinationPath.Trim() == string.Empty)
            return null;

        //create directory
        var fullPath = Path.Combine(UserHelper.GetRootPathForUser(ownerId), destinationPath, fileName);
        if (!Directory.Exists(fullPath))
        {
            return null;
        }
        if (File.Exists(fullPath))
        {
            return null;
        }
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
            return _context.FileItems.Where(f => f.OwnerID == ownerId &&
            f.Path.StartsWith(UserHelper.GetRootPathForUser(ownerId))).ToList();
        }

        return _context.FileItems.Where(f => f.OwnerID == ownerId &&
            f.Path.StartsWith(Path.Combine(UserHelper.GetRootPathForUser(ownerId), parentPath))).ToList();
    }

    public async Task<Stream?> DownloadFileAsync(Guid fileId, Guid userId)
    {
        if (fileId == Guid.Empty || userId == Guid.Empty)
            return null;

        var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
        if (fileItem == null || fileItem.IsFolder)
            return null;

        if (!File.Exists(fileItem.Path))
            return null;

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fileItem.Path, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        return memoryStream;
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

        var destinationFullPath = Path.Combine(UserHelper.GetRootPathForUser(userId), newPath, fileItem.Name);
        if (fileItem.IsFolder)
        {
            if (Directory.Exists(destinationFullPath))
            {
                return null;
            }
            Directory.Move(fileItem.Path, destinationFullPath);

            var movedItems = _context.FileItems.Where(f => f.Path == fileItem.Path ||
                f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar)).ToList();
            foreach (var item in movedItems)
            {
                item.Path = item.Path.Replace(fileItem.Path, destinationFullPath);
            }
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

        var fileItem = _context.FileItems.Where(f => f.ID == fileId && f.OwnerID == userId).FirstOrDefault();
        if (fileItem == null)
            return null;

        var newFullPath = Path.GetDirectoryName(fileItem.Path) + Path.DirectorySeparatorChar + newName;
        if (fileItem.IsFolder)
        {
            if (Directory.Exists(newFullPath))
            {
                return null;
            }
            Directory.Move(fileItem.Path, newFullPath);

            var renamedItems = _context.FileItems.Where(f => f.Path == fileItem.Path ||
                f.Path.StartsWith(fileItem.Path + Path.DirectorySeparatorChar)).ToList();
            foreach (var item in renamedItems)
            {
                item.Path = item.Path.Replace(fileItem.Path, newFullPath);
            }
            fileItem.Name = newName;
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