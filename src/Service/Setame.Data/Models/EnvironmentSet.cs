﻿namespace Setame.Data.Models;

public record EnvironmentSetCreated(Guid Id, string Name);

public record EnvironmentSetRenamed(Guid Id, string NewName);

public record EnvironmentAdded(string Name);

public record EnvironmentRemoved(string Name);

public record EnvironmentRenamed(string OldName, string NewName);

public record EnvironmentSetDeleted(Guid Id);

public record EnvironmentSetVariableAdded(string Name);

public record EnvironmentSetVariableChanged(string Environment, string VariableName, string NewValue);

public record EnvironmentSetVariableRenamed(string VariableName, string NewName);
public record EnvironmentSetVariableDeleted(string VariableName);

public record ApplicationAssociatedToEnvironmentSet(Guid ApplicationId, Guid EnvironmentSetId);

public class EnvironmentSet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VariableGrid Settings { get; set; } = new();
    public List<DeploymentEnvironment> Environments { get; set; } = new();
    public List<Guid> Applications { get; set; } = new();
    public long Version { get; set; }

    public void Apply(EnvironmentSetCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Environments = new List<DeploymentEnvironment>();
    }

    public void Apply(EnvironmentAdded e)
    {
        Environments.Add(new DeploymentEnvironment { Name = e.Name });
    }

    public void Apply(EnvironmentRemoved e)
    {
        Environments.RemoveAll(x => x.Name == e.Name);
    }

    public void Apply(EnvironmentSetVariableAdded e)
    {
        foreach (var env in Environments)
        {
            env.Settings.Add(e.Name, "");
            Settings[e.Name, env.Name] = "";
        }
    }

    public void Apply(EnvironmentSetRenamed e)
    {
        Name = e.NewName;
    }

    public void Apply(EnvironmentSetVariableChanged e)
    {
        var environment = Environments.First(x => x.Name == e.Environment);
        environment.Settings[e.VariableName] = e.NewValue;
        Settings[e.VariableName, e.Environment] = e.NewValue;
    }

    public void Apply(EnvironmentSetVariableRenamed e)
    {
            foreach (var environment in Environments)
            {
                var value = environment.Settings[e.VariableName];
                environment.Settings.Remove(e.VariableName);
                environment.Settings.Add(e.NewName, value);
            }

            Settings.RenameVariable(e.VariableName, e.NewName);
    }

    public void Apply(ApplicationAssociatedToEnvironmentSet e)
    {
        Applications.Add(e.ApplicationId);
    }

    public void Apply(EnvironmentSetDeleted e)
    {
    }

    public void Apply(EnvironmentRenamed e)
    {
        Environments.First(x => x.Name == e.OldName).Name = e.NewName;
        Settings.RenameEnvironment(e.OldName, e.NewName);
    }

    public void Apply(EnvironmentSetVariableDeleted e)
    {
        foreach (var environment in Environments)
        {
            environment.Settings.Remove(e.VariableName);
        }

     //   Settings.RemoveVariable(e.VariableName);
    }
}