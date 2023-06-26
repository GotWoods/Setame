using Marten.Linq.SoftDeletes;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace ConfigMan.Data.Models;

public record EnvironmentSetCreated(Guid Id, string Name);
public record EnvironmentSetRenamed(string NewName);
public record EnvironmentAdded(string Name);
public record EnvironmentRemoved(string Name);
public record EnvironmentSetDeleted();
public record EnvironmentSetVariableAdded(string Name);
public record EnvironmentSetVariableChanged(string Environment, string VariableName, string NewValue);
public record EnvironmentSetVariableRenamed(string VariableName, string NewName);

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
        //TODO: Would this need to clone all variables into the environment then?
        this.DeploymentEnvironments.Add(new DeploymentEnvironment() { Name=e.Name });
    }

    public void Apply(EnvironmentRemoved e)
    {
        this.DeploymentEnvironments.RemoveAll(x => x.Name == e.Name);
    }

    public void Apply(EnvironmentSetVariableAdded e)
    {
        foreach (var env in this.DeploymentEnvironments)
        {
            env.EnvironmentSettings.Add(e.Name, "");
        }
    }

    public void Apply(EnvironmentSetRenamed e)
    {
        this.Name = e.NewName;
    }

    public void Apply(EnvironmentSetVariableChanged e)
    {
        var environment = this.DeploymentEnvironments.First(x=>x.Name == e.Environment);
        environment.EnvironmentSettings[e.VariableName] = e.NewValue;
    }

    public void Apply(EnvironmentSetVariableRenamed e)
    {
        foreach (var environment in DeploymentEnvironments)
        {
            var value = environment.EnvironmentSettings[e.VariableName];
            environment.EnvironmentSettings.Remove(e.VariableName);
            environment.EnvironmentSettings.Add(e.NewName, value);
        }
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