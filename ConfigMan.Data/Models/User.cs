using System.ComponentModel.DataAnnotations;

namespace ConfigMan.Data.Models;

public class User
{
    [Key] public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
}