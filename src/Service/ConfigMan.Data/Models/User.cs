using System.ComponentModel.DataAnnotations;

namespace ConfigMan.Data.Models;

public record UserCreated(Guid Id, string Username, string PasswordHash, string Salt);

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;

    public void Apply(UserCreated e)
    {
        this.Id = e.Id;
        this.Username = e.Username;
        this.PasswordHash = e.PasswordHash;
        this.Salt = e.Salt;
    }
}