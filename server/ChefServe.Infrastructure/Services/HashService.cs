using ChefServe.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ChefServe.Core.Services;

public class HashService : IHashService
{
    public string ComputeHash(string input)
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        string salt = Convert.ToBase64String(saltBytes);


        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(input + salt);
        var hash = sha256.ComputeHash(combined);
        string hashString = Convert.ToBase64String(hash);
        return hashString + ":" + salt;
    }
    
    public bool VerifyHash(string input, string hash)
    {
        var parts = hash.Split(':');
        if (parts.Length != 2)
            return false;

        string storedHash = parts[0];
        string storedSalt = parts[1];

        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(input + storedSalt);
        var computedHash = Convert.ToBase64String(sha256.ComputeHash(combined));

        return computedHash == storedHash;
    }
}
