using Marten.Linq.SoftDeletes;

namespace ConfigMan.Data.Models;

public record EnvironmentSetCreated(Guid Id, string Name);
public record EnvironmentSetRenamed(Guid Id, string NewName);
public record EnvironmentAdded(string Name);
public record EnvironmentRemoved(string Name);
public record EnvironmentRenamed(string OldName, string NewName);
public record EnvironmentSetDeleted(Guid Id);
public record EnvironmentSetVariableAdded(string Name);
public record EnvironmentSetVariableChanged(string Environment, string VariableName, string NewValue);
public record EnvironmentSetVariableRenamed(string VariableName, string NewName);
public record ApplicationAssociatedToEnvironmentSet(Guid ApplicationId, Guid EnvironmentSetId);

public class EnvironmentSet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DeploymentEnvironment> DeploymentEnvironments { get; set; } = new();
    public List<Guid> Applications { get; set; } = new();
    public long Version { get; set; }

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

    public void Apply(ApplicationAssociatedToEnvironmentSet e)
    {
        this.Applications.Add(e.ApplicationId);
    }

    public void Apply(EnvironmentSetDeleted e)
    {

    }

    public void Apply(EnvironmentRenamed e)
    {
        DeploymentEnvironments.First(x => x.Name == e.OldName).Name = e.NewName;
    }
}