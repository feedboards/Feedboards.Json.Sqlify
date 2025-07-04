using System.Text.Json;
using Feedboards.Json.Sqlify.Extensions;

namespace Feedboards.Json.Sqlify.JSON.PostgresSql;

public class PostgresSqlJsonAnalyzer
{
    private readonly PostgresSqlTypeDetector postgresSqlTypeDetector = new();
    private readonly PostgresSqlObjectComparer postgresSqlObjectComparer = new();
    
    public Dictionary<string, Dictionary<string, string>> AnalyzeJsonStructure(
        JsonElement jsonData,
        string prefix,
        string tableName = "root")
    {
        return AnalyzeJsonStructure(jsonData, prefix, tableName, null);
    }

    private Dictionary<string, Dictionary<string, string>> AnalyzeJsonStructure(
        JsonElement jsonData,
        string prefix,
        string tableName,
        string? fk,
        string? fkType = null)
    {
        var structure = new Dictionary<string, Dictionary<string, string>>();

        if (!structure.ContainsKey(tableName))
        {
            structure[tableName] = new Dictionary<string, string>();
        }

        if (fk != null && fkType != null)
        {
            structure[tableName][fk] = fkType;
        }

        if (jsonData.ValueKind == JsonValueKind.Array)
        {
            // One to many ref
            var arr = jsonData.EnumerateArray().ToList();

            if (arr.Count > 0)
            {
                postgresSqlObjectComparer.SumUpArrays(arr, tableName);
                
                // if (fk != null && fkType != null)
                // {
                //     structure[tableName]["id"] = "SERIAL PRIMARY KEY";
                //     structure[tableName][$"{fk}_id"] = fkType; //TODO test
                //     structure[tableName]["value"] = postgresSqlTypeDetector.DetectNullableType(
                //         jsonData,
                //         postgresSqlTypeDetector.DetectType(jsonData));
                // }
                // else
                // {
                //     //First level
                //     //TODO fix it
                //     structure[tableName]["id"] = "SERIAL PRIMARY KEY";
                //     structure[tableName]["value"] = postgresSqlTypeDetector.DetectNullableType(
                //         jsonData,
                //         postgresSqlTypeDetector.DetectType(jsonData));
                // }
            }
            else
            {
                //Empty array
                //TODO come up with idea of how to handle that
            }
        }
        else if (jsonData.ValueKind == JsonValueKind.Object)
        {
            // One to one ref
            //TODO
            foreach (var prop in jsonData.EnumerateObject())
            {
                var safeKey = prop.Name.Replace(" ", "_");
                var fieldPath = string.IsNullOrEmpty(prefix) 
                    ? safeKey 
                    : $"{prefix}.{safeKey}";
                
                
                
                if (!structure.ContainsKey(fieldPath))
                {
                    structure[fieldPath] = new Dictionary<string, string>();
                }
                
                var result = AnalyzeJsonStructure(prop.Value, prefix, tableName, prop.Name);
                
                structure.MergeNestedDictionaries(result);
            }
        }
        else
        {
            // Handle simple values at root level
            structure[tableName][prefix] = postgresSqlTypeDetector.DetectNullableType(
                jsonData,
                postgresSqlTypeDetector.DetectType(jsonData));
        }
        
        return structure;
    }
}