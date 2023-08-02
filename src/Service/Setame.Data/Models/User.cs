namespace Setame.Data.Models;

public record UserCreated(Guid Id, string Username, string PasswordHash, string Salt);
public record PasswordResetRequested(string Token, DateTime Expiration, Guid UserId);
public record PasswordReset(string PasswordHash, string Salt);
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string PasswordResetToken { get; set; } = string.Empty;
    public DateTime PasswordResetTokenExpiration { get; set; } = DateTime.MinValue;

    public void Apply(UserCreated e)
    {
        this.Id = e.Id;
        this.Username = e.Username;
        this.PasswordHash = e.PasswordHash;
        this.Salt = e.Salt;
    }

    public void Apply(PasswordResetRequested e)
    {
        this.PasswordResetToken = e.Token;
        this.PasswordResetTokenExpiration = e.Expiration;
    }

    public void Apply(PasswordReset e)
    {
        this.PasswordHash = e.PasswordHash;
        this.Salt = e.Salt;
        this.PasswordResetToken = string.Empty;
        this.PasswordResetTokenExpiration = DateTime.MinValue;
    }
}