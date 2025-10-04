using ChefServe.Core.Models;
using ChefServe.Infrastructure.Data;
namespace ChefServe.Core.Helper;

public class UserHelper
{
    public static string GetRootPathForUser(Guid userId)
    {
        var relativePath = $"../Users/{userId}";
        var fullPath = Path.GetFullPath(relativePath);
        return fullPath;
    }
}