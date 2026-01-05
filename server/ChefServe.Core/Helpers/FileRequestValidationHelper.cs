using ChefServe.Core.DTOs;

namespace ChefServe.API.Validators;

public static class CreateFolderValidator
{
    public static (bool IsValid, string? Error) Validate(CreateFolderBodyDTO dto)
    {
        if (dto == null)
            return (false, "Request must contain a body.");

        if (string.IsNullOrWhiteSpace(dto.FolderName))
            return (false, "Missing folder name.");

        // Optional: default ParentPath to empty string
        if (dto.ParentPath == null)
            dto.ParentPath = "";

        return (true, null);
    }
}

public static class UploadFileValidator
{
    public static (bool IsValid, string? Error) Validate(UploadFileFormDTO dto)
    {
        if (dto == null)
            return (false, "Request must contain a body.");

        if (string.IsNullOrWhiteSpace(dto.FileName))
            return (false, "Missing file name.");

        if (dto.Content == null || dto.Content.Length == 0)
            return (false, "File content is missing or empty.");

        // Optional: default DestinationPath to empty string
        if (dto.DestinationPath == null)
            dto.DestinationPath = "";

        return (true, null);
    }
}

public static class RenameFileValidator
{
    public static (bool IsValid, string? Error) Validate(RenameFileBodyDTO dto)
    {
        if (dto == null)
            return (false, "Request must contain a body.");

        if (dto.FileID == Guid.Empty)
            return (false, "Missing file ID.");

        if (string.IsNullOrWhiteSpace(dto.NewName))
            return (false, "Missing new file name.");

        return (true, null);
    }
}



