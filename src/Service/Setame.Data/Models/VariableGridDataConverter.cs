using System.Text.Json;
using System.Text.Json.Serialization;

namespace Setame.Data.Models;

public class VariableGridDataConverter : JsonConverter<VariableGrid>
{
    public override VariableGrid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Deserialization is not supported for GridData.");
    }

    public override void Write(Utf8JsonWriter writer, VariableGrid value, JsonSerializerOptions options)
    {
        var gridArray = new List<List<string>>();
        var variableNames = new List<string>();
        var environmentNames = new List<string>();

        foreach (var variableName in value.GetVariableNames())
        {
            variableNames.Add(variableName);
            var variableData = value.GetVariableData(variableName);
            foreach (var environmentName in variableData.Keys)
            {
                if (!environmentNames.Contains(environmentName))
                {
                    environmentNames.Add(environmentName);
                }
            }
        }

        // Add environment names as the first row in the grid array
        gridArray.Add(environmentNames.ToList());

        // Add variable names and data to the grid array
        foreach (var variableName in variableNames)
        {
            var variableData = value.GetVariableData(variableName);
            var row = new List<string> { variableName };
            foreach (var environmentName in environmentNames)
            {
                if (variableData.TryGetValue(environmentName, out var valueForEnvironment))
                {
                    row.Add(valueForEnvironment);
                }
                else
                {
                    row.Add(null); // or any default value for missing data
                }
            }
            gridArray.Add(row);
        }

        JsonSerializer.Serialize(writer, gridArray, options);
    }
}