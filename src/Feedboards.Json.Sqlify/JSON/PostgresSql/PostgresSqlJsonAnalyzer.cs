using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgresSql;

public class PostgresSqlJsonAnalyzer
{
    private readonly PostgresSqlTypeDetector PostgresSqlTypeDetector = new();
    
    public Dictionary<string, Dictionary<string, string>> AnalyzeJsonStructure(JsonElement jsonData, string prefix)
    {
        var structure = new Dictionary<string, Dictionary<string, string>>();
        var tableName = "root";

        if (jsonData.ValueKind == JsonValueKind.Array)
        {
            // TODO
        }
        else if (jsonData.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in jsonData.EnumerateObject())
            {
                var safeKey = prop.Name.Replace(" ", "_");
                var fieldPath = string.IsNullOrEmpty(prefix) ? safeKey : $"{prefix}.{safeKey}";

                if (!structure.ContainsKey(fieldPath))
                {
                    structure[fieldPath] = new Dictionary<string, string>();
                }

                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    //TODO create method which will return table name by rules
                    continue;
                }
                else if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    // TODO
                    continue;
                }
                
                // Handle simple values
                structure[tableName][fieldPath] = PostgresSqlTypeDetector.DetectNullableType(prop.Value,
                    PostgresSqlTypeDetector.DetectType(prop.Value));
            }
        }
        else
        {
            // Handle simple values at root level
            if (!structure.ContainsKey(prefix))
            {
                structure[prefix] = new Dictionary<string, string>();
            }
            structure[tableName][prefix] = PostgresSqlTypeDetector.DetectType(jsonData) ?? "VARCHAR";
        }
        
        return structure;
    }
}