using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Setame.Data.Models;

[JsonConverter(typeof(VariableGridDataConverter))]
public class VariableGrid
{
    private readonly Dictionary<string, Dictionary<string, string?>> data = new Dictionary<string, Dictionary<string, string?>>();

    public string? this[string variableName, string environment]
    {
        get
        {
            if (data.TryGetValue(variableName, out var variableData))
            {
                if (variableData.TryGetValue(environment, out var value))
                {
                    return value;
                }
            }
            return null; // or throw an exception, or handle the case when the data is not found
        }
        set
        {
            if (!data.ContainsKey(variableName))
            {
                data[variableName] = new Dictionary<string, string?>();
            }

            data[variableName][environment] = value;
        }
    }

    public void RenameVariable(string oldVariableName, string newVariableName)
    {
        if (data.TryGetValue(oldVariableName, out var variableData))
        {
            data[newVariableName] = variableData;
            data.Remove(oldVariableName);
        }
        else
        {
            // Handle the case when the old variable name is not found
            // For example, you can throw an exception or perform appropriate error handling.
        }
    }

    public IEnumerable<string> GetVariableNames()
    {
        return data.Keys;
    }

    public Dictionary<string, string?> GetVariableData(string variableName)
    {
        if (data.TryGetValue(variableName, out var variableData))
        {
            return variableData;
        }

        return new Dictionary<string, string?>(); // or throw an exception, or handle the case when the variable name is not found
    }

    public void RenameEnvironment(string oldEnvironmentName, string newEnvironmentName)
    {
        foreach (var variableName in data.Keys.ToList())
        {
            if (data.TryGetValue(variableName, out var variableData))
            {
                if (variableData.TryGetValue(oldEnvironmentName, out var value))
                {
                    variableData[newEnvironmentName] = value;
                    variableData.Remove(oldEnvironmentName);
                }
                else
                {
                    // Handle the case when the old environment name is not found for a specific variable
                    // For example, you can log the issue or perform appropriate error handling.
                }
            }
            else
            {
                // Handle the case when the variable name is not found (unlikely in this scenario, but included for completeness)
                // For example, you can log the issue or perform appropriate error handling.
            }
        }
    }

    public Dictionary<string, string?> GetVariablesForEnvironment(string environment)
    {
        var environmentData = new Dictionary<string, string?>();

        foreach (var variableEntry in data)
        {
            if (variableEntry.Value.TryGetValue(environment, out var value))
            {
                environmentData[variableEntry.Key] = value;
            }
        }

        return environmentData;
    }

}
