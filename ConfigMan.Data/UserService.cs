using System.Security.Cryptography;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigMan.Data;

public interface IUserService
{
    Task CreateUserAsync(User user, string password);
    Task<User?> GetUserByUsernameAsync(string username);
    bool VerifyPassword(User user, string password);
}

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateUserAsync(User user, string password)
    {
        // Generate a new salt
        user.Salt = GenerateSalt();
        // Hash the password with the salt
        user.PasswordHash = HashPassword(password, user.Salt);

        // Save the user to the database
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    private string GenerateSalt()
    {
        var rng = new RNGCryptoServiceProvider();
        var salt = new byte[32];
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    private string HashPassword(string password, string salt)
    {
        using var hasher = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000);
        var hash = hasher.GetBytes(32);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(User user, string password)
    {
        // Hash the provided password with the user's salt
        var hashedPassword = HashPassword(password, user.Salt);

        // Compare the provided hashed password with the stored password hash
        return hashedPassword == user.PasswordHash;
    }
}