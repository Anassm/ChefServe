namespace ChefServe.Core.DTOs;

public class FileItemDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? ParentFolderId { get; set; }
    public bool IsFolder { get; set; }
}

public class CreateFolderDto
{
    public string FolderName { get; set; } = string.Empty;
    public int? ParentFolderId { get; set; }
}

public class MoveFileDto
{
    public int FileId { get; set; }
    public int? NewParentFolderId { get; set; }
}

public class RenameFileDto
{
    public int FileId { get; set; }
    public string NewName { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}