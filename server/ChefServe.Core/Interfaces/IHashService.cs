using ChefServe.Core.Models;

namespace ChefServe.Core.Interfaces;

public interface IHashService
{
    string ComputeHash(string input);
    bool VerifyHash(string input, string hash);
}