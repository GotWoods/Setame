namespace Setame.Web.Models;

public class AppLoginRequest
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
}