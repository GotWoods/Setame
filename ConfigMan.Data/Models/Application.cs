using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigMan.Data.Models;

public record ApplicationCreated(Guid Id, string Name, string Token, Guid EnvironmentSet);
public record ApplicationEnvironmentAdded(string Name);
public record ApplicationRenamed(string NewName);
public record ApplicationVariableAdded(string Name);
public record ApplicationVariableChanged(string Environment, string VariableName, string NewValue);
public record ApplicationDefaultVariableAdded(string VariableName);
public record ApplicationDefaultVariableChanged(string VariableName, string NewValue);
public record ApplicationVariableRenamed(string VariableName, string NewName);

public class Application
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public Guid EnvironmentSetId { get; set; }
    public List<Setting> ApplicationDefaults { get; set; } = new();
    public List<Environment> EnvironmentSettings { get; set; } = new();
    

    public void Apply(ApplicationCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Token = e.Token;
        EnvironmentSetId = e.EnvironmentSet;
    }

    public void Apply(ApplicationRenamed e)
    {
        Name = e.NewName;
    }

    public void Apply(ApplicationVariableAdded e)
    {
        foreach (var env in EnvironmentSettings) env.Settings.Add(new Setting { Name = e.Name });
        //foreach (var env in EnvironmentSettings.Keys) EnvironmentSettings[env].Add(new Setting { Name = e.Name });
    }

    public void Apply(ApplicationVariableChanged e)
    {
        EnvironmentSettings.First(x => x.Name == e.Environment).Settings.Find(x => x.Name == e.VariableName).Value = e.NewValue;
        //EnvironmentSettings[e.Environment].First(x => x.Name == e.VariableName).Value = e.NewValue;
    }

    public void Apply(ApplicationVariableRenamed e)
    {
        foreach (var env in EnvironmentSettings) env.Settings.First(x => x.Name == e.VariableName).Name = e.NewName;
        //foreach (var env in EnvironmentSettings.Keys) EnvironmentSettings[env].First(x => x.Name == e.VariableName).Name = e.NewName;
    }

    public void Apply(ApplicationEnvironmentAdded e)
    {
        EnvironmentSettings.Add(new Environment() { Name=e.Name });
        //EnvironmentSettings.Add(e.Name, new List<Setting>());
    }


    public void Apply(ApplicationDefaultVariableAdded e)
    {
        ApplicationDefaults.Add(new Setting() { Name=e.VariableName });
    }

    public void Apply(ApplicationDefaultVariableChanged e)
    {
        ApplicationDefaults.First(x => x.Name == e.VariableName).Value = e.NewValue;
    }

    // public Dictionary<string, string> GetAppliedSettings(DeploymentEnvironment environment)
    // {
    //     var appliedSettings = new Dictionary<string, string>();
    //     //foreach (var setting in environment.Settings) appliedSettings.Add(setting.Name, setting.Value);
    //     foreach (var setting in ApplicationDefaults) appliedSettings[setting.Name] = setting.Value;
    //
    //     if (EnvironmentSettings.ContainsKey(environment.Name))
    //         foreach (var setting in EnvironmentSettings[environment.Name])
    //             if (!appliedSettings.ContainsKey(setting.Name))
    //                 appliedSettings.Add(setting.Name, setting.Value);
    //             else
    //                 appliedSettings[setting.Name] = setting.Value;
    //     return appliedSettings;
    // }
}