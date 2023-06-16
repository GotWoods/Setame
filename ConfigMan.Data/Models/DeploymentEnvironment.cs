using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigMan.Data.Models;

public class DeploymentEnvironment
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Dictionary<string, string> EnvironmentSettings { get; set; } = new();
}

