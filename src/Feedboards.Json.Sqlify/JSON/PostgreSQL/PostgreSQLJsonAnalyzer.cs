using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgreSQL;

public class PostgreSQLJsonAnalyzer
{
    public readonly PostgreSQLTypeDetector PostgreSQLTypeDetector = new();
    
    public Dictionary<string, List<Dictionary<string, string>>> AnalyzeJsonStructure(JsonElement jsonData, string prefix)
    {
        var structure = new Dictionary<string, List<Dictionary<string, string>>>();

        if (jsonData.ValueKind != JsonValueKind.Array && string.IsNullOrEmpty(prefix))
        {
            // TODO
        }
        else if (jsonData.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in jsonData.EnumerateObject())
            {
                var safeKey = prop.Name.Replace(" ", "_");
                var fieldPath = string.IsNullOrEmpty(prefix) ? safeKey : $"{prefix}.{safeKey}";
                var value = prop.Value;

                if (value.ValueKind == JsonValueKind.Array)
                {
                    //TODO create method which will return table name by rules
                    var nestedTable = fieldPath;
                    
                    var nestedDefs = AnalyzeJsonStructure(value, nestedTable);
                    
                    continue;
                }
                else if (value.ValueKind == JsonValueKind.Object)
                {
                    continue;
                }
                
                
            }
        }
        else
        {
            // Handle simple values at root level
            // TODO
        }
        
        return structure;
    }
}