using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigMan.Data.Models;

public class Application
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public List<Setting>? ApplicationDefaults { get; set; } = new();

    [Column(TypeName = "jsonb")]
    public Dictionary<string, List<Setting>> EnvironmentSettings { get; set; } = new();
    
    public Dictionary<string, string> GetAppliedSettings(DeploymentEnvironment environment)
    {
        var appliedSettings = new Dictionary<string, string>();
        foreach (var setting in environment.Settings) appliedSettings.Add(setting.Name, setting.Value);
        foreach (var setting in ApplicationDefaults) appliedSettings[setting.Name] = setting.Value;

        if (EnvironmentSettings.ContainsKey(environment.Name))
        {
            foreach (var setting in EnvironmentSettings[environment.Name])
            {
                if (!appliedSettings.ContainsKey(setting.Name))
                    appliedSettings.Add(setting.Name, setting.Value);
                else
                    appliedSettings[setting.Name] = setting.Value;
            }
        }
        return appliedSettings;
    }
}