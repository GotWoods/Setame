namespace ConfigMan.Data.Models;

public class EnvironmentSet
{
    public string Name { get; set; } = string.Empty;
    
    public List<DeploymentEnvironment> DeploymentEnvironments { get; set; } = new();
}