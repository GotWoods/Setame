using System.ComponentModel.DataAnnotations;

namespace ConfigMan.Data.Models;

public record UserCreated(Guid Id, string Username, string PasswordHash, string Salt);

public class User
{
    [Key] public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }

    public void Apply(UserCreated e)
    {
        this.Id = e.Id;
        this.Username = e.Username;
        this.PasswordHash = e.PasswordHash;
        this.Salt = e.Salt;
    }
}