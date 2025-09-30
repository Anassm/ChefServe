using ChefServe.Core.Models;
using ChefServe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ChefServe.Core.Interfaces;
public class UserServices
{
    private readonly ChefServeDbContext _context;
    public UserServices(ChefServeDbContext context)
    {
        _context = context;
    }
    public User GetUserById(Guid userId)
    {
        var user = _context.Users.Find(userId);
        return user;
    }
}