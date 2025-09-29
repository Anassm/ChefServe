using ChefServe.Core.Models;
namespace ChefServe.Core.DTOs;

public class UploadFileDTO
{
    public Guid OwnerID { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
}