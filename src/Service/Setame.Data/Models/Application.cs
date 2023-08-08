namespace Setame.Data.Models;

public interface IApplicationEvent { }

public record ApplicationCreated(Guid Id, string Name, string Token, Guid EnvironmentSetId) : IApplicationEvent;

public record ApplicationEnvironmentAdded(string Name) : IApplicationEvent;

public record ApplicationRenamed(string NewName) : IApplicationEvent;

public record ApplicationVariableAdded(string Environment, string Name) : IApplicationEvent;

public record ApplicationVariableChanged(string Environment, string VariableName, string NewValue) : IApplicationEvent;

public record ApplicationDefaultVariableAdded(string VariableName) : IApplicationEvent;

public record ApplicationDefaultVariableChanged(string VariableName, string NewValue) : IApplicationEvent;
public record ApplicationVariableDeleted(string VariableName);

public record ApplicationVariableRenamed(string VariableName, string NewName) : IApplicationEvent;
public record ApplicationDefaultVariableRenamed(string VariableName, string NewName) : IApplicationEvent;

public record ApplicationDeleted(Guid ApplicationId) : IApplicationEvent;

public class Application
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public Guid EnvironmentSetId { get; set; }
    public List<Setting> ApplicationDefaults { get; set; } = new();
    public List<Environment> EnvironmentSettings { get; set; } = new();
    public int Version { get; set; }

    public void Apply(ApplicationCreated e)
    {
        Id = e.Id;
        Name = e.Name;
        Token = e.Token;
        EnvironmentSetId = e.EnvironmentSetId;
    }

    public void Apply(ApplicationRenamed e)
    {
        Name = e.NewName;
    }

    public void Apply(ApplicationVariableAdded e)
    {
        EnvironmentSettings.First(x => x.Name == e.Environment).Settings.Add(new Setting { Name = e.Name });
    }

    public void Apply(ApplicationVariableChanged e)
    {
        EnvironmentSettings.First(x => x.Name == e.Environment).Settings.Find(x => x.Name == e.VariableName)!.Value =
            e.NewValue;
    }

    public void Apply(ApplicationVariableRenamed e)
    {
        foreach (var env in EnvironmentSettings) env.Settings.First(x => x.Name == e.VariableName).Name = e.NewName;
    }

    public void Apply(ApplicationDefaultVariableRenamed e)
    {
        ApplicationDefaults.First(x => x.Name == e.VariableName).Name = e.NewName;
    }

    public void Apply(ApplicationEnvironmentAdded e)
    {
        var environment = new Environment { Name = e.Name };
        var template = EnvironmentSettings.FirstOrDefault(); //use this to copy all variable names to the new environment 
        if (template != null)
            foreach (var setting in template.Settings)
                environment.Settings.Add(new Setting { Name = setting.Name });
        EnvironmentSettings.Add(environment);
    }


    public void Apply(ApplicationDefaultVariableAdded e)
    {
        ApplicationDefaults.Add(new Setting { Name = e.VariableName });
    }

    public void Apply(ApplicationDefaultVariableChanged e)
    {
        ApplicationDefaults.First(x => x.Name == e.VariableName).Value = e.NewValue;
    }

    public void Apply(EnvironmentRenamed e)
    {
        EnvironmentSettings.First(x => x.Name == e.OldName).Name = e.NewName;
    }

    public void Apply(EnvironmentRemoved e)
    {
        foreach (var environmentSetting in EnvironmentSettings)
            environmentSetting.Settings.RemoveAll(x => x.Name == e.Name);
    }

    public void Apply(ApplicationVariableDeleted e)
    {
        foreach (var environmentSetting in EnvironmentSettings)
            environmentSetting.Settings.RemoveAll(x => x.Name == e.VariableName);
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