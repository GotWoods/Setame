namespace Setame.ConfigurationProvider;

public class AppLoginRequest
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Token { get; set; } = String.Empty;
    public string Environment { get; set; } = String.Empty;
}