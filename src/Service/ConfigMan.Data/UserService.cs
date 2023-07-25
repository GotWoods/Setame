using System.Security.Cryptography;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using JasperFx.Core;
using Marten;

namespace ConfigMan.Data;

public interface IUserService
{
    Task CreateUserAsync(User user, string password);
    UserSummary? GetUserByUsernameAsync(string username);
    bool VerifyPassword(UserSummary user, string password);
}

public class UserService : IUserService
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;

    public UserService(IDocumentSession documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task CreateUserAsync(User user, string password)
    {
        // Generate a new salt
        user.Salt = GenerateSalt();
        // Hash the password with the salt
        user.PasswordHash = HashPassword(password, user.Salt);

        var id = CombGuidIdGeneration.NewGuid();
        _documentSession.Events.StartStream<User>(id, new UserCreated(id, user.Username, user.PasswordHash, user.Salt));
        await _documentSession.SaveChangesAsync();
    }

    public UserSummary? GetUserByUsernameAsync(string username)
    {
        return _querySession.Query<UserSummary>().FirstOrDefault(x => x.Username == username);
    }

    private string GenerateSalt()
    {
        var rng = RandomNumberGenerator.Create();
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

    public bool VerifyPassword(UserSummary user, string password)
    {
        // Hash the provided password with the user's salt
        var hashedPassword = HashPassword(password, user.Salt);

        // Compare the provided hashed password with the stored password hash
        return hashedPassword == user.PasswordHash;
    }
}