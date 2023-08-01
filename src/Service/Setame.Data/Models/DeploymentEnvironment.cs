namespace Setame.Data.Models;

public class DeploymentEnvironment
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Dictionary<string, string> Settings { get; set; } = new();
}

