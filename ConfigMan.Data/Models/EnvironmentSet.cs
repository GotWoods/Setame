namespace ConfigMan.Data.Models;

public class EnvironmentSet
{
    public string Name { get; set; } = string.Empty;
    
    public List<DeploymentEnvironment> DeploymentEnvironments { get; set; } = new();

    public EnvironmentSet Copy()
    {
        var copy = (EnvironmentSet)this.MemberwiseClone();
        copy.DeploymentEnvironments = this.DeploymentEnvironments; //as this is just a JSON object, this should be ok not to create unique objects

        //copy.DeploymentEnvironments = new List<DeploymentEnvironment>();
        // foreach (var environment in DeploymentEnvironments)
        // {
        //     copy.DeploymentEnvironments.Add(environment.Copy());
        // }

        return copy;
    }
}