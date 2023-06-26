using Marten.Linq.SoftDeletes;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace ConfigMan.Data.Models;

public record EnvironmentSetRenamed(string OldName, string NewName);
public record EnvironmentAdded(string Name);
public record EnvironmentRemoved(string Name);
public record EnvironmentSetCreated(Guid Id, string Name);

public class EnvironmentSet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DeploymentEnvironment> DeploymentEnvironments { get; set; } = new();
    
    public void Apply(EnvironmentSetCreated e)
    {
        this.Id = e.Id;
        this.Name = e.Name;
        this.DeploymentEnvironments = new List<DeploymentEnvironment>();
    }

    public void Apply(EnvironmentAdded e)
    {
        this.DeploymentEnvironments.Add(new DeploymentEnvironment() { Name=e.Name });
    }

    public void Apply(EnvironmentRemoved e)
    {
        this.DeploymentEnvironments.RemoveAll(x => x.Name == e.Name);
    }


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