using System.Text.Json;
using System.Text.Json.Serialization;

namespace Setame.Data.Models;

public class VariableGridDataConverter : JsonConverter<VariableGrid>
{
    public override VariableGrid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var gridArray = JsonSerializer.Deserialize<List<List<string>>>(ref reader, options);
        if (gridArray == null || gridArray.Count == 0)
        {
            throw new JsonException("Invalid data format.");
        }

        var variableGrid = new VariableGrid();

        var environmentNames = gridArray[0];
        for (int i = 1; i < gridArray.Count; i++)
        {
            var row = gridArray[i];
            if (row.Count == 0)
            {
                continue; // skip empty rows
            }

            var variableName = row[0];
            for (int j = 1; j < row.Count && j <= environmentNames.Count; j++)
            {
                var environmentName = environmentNames[j - 1];
                var value = row[j];
                variableGrid[variableName, environmentName] = value;
            }
        }

        return variableGrid;
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