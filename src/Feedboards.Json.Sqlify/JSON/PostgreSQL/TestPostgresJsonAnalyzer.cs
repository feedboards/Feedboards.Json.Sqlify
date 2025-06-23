using System.Text;
using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgreSQL;

public class TestPostgresJsonAnalyzer
{
    private readonly TestPostgresTypeDetector typeDetector = new();

    public string GenerateDDL(JsonElement element, string rootTableName = "root", string parentId = null)
    {
        var sb = new StringBuilder();
        
        var tableDefinitions = Analyze(element, rootTableName, parentId);
        
        foreach (var (tableName, rows) in tableDefinitions)
        {
            var columns = rows
                .SelectMany(d => d)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.First().Value);

            sb.AppendLine($"CREATE TABLE {tableName} (");
            sb.AppendLine("    id SERIAL PRIMARY KEY,");

            foreach (var col in columns)
            {
                if (col.Key == "id")
                {
                    continue;
                }
                
                sb.AppendLine($"    {col.Key} {col.Value},");
            }
            
            sb.Length -= 3;
            sb.AppendLine("\n);\n");
        }
        return sb.ToString();
    }
    
    private Dictionary<string, List<Dictionary<string, string>>> Analyze(
        JsonElement element,
        string tableName = "root",
        string parentId = null)
    {
        var tableDefinitions = new Dictionary<string, List<Dictionary<string, string>>>();
        var row = new Dictionary<string, string>();

        if (element.ValueKind == JsonValueKind.Array)
        {
            //TODO
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                var key = prop.Name;
                var value = prop.Value;

                if (value.ValueKind == JsonValueKind.Object)
                {
                    string nestedTable = $"{tableName}_{key}";
                    var nestedDefs = Analyze(value, nestedTable, "id");
                    MergeTableDefinitions(tableDefinitions, nestedDefs);
                    row[$"{key}_id"] = "INT";
                }
                else if (value.ValueKind == JsonValueKind.Array)
                {
                    string nestedTable = $"{tableName}_{key}";
                    foreach (var item in value.EnumerateArray())
                    {
                        var nestedDefs = Analyze(item, nestedTable, "id");
                        MergeTableDefinitions(tableDefinitions, nestedDefs);
                    }
                }
                else
                {
                    row[key] = typeDetector.DetectType(value);
                }
            }
        }
        else
        {
            // Handle simple values at root level
            //TODO 
        }

        if (parentId != null)
        {
            row[$"{tableName}_parent_id"] = "INT";
        }

        if (!tableDefinitions.ContainsKey(tableName))
            tableDefinitions[tableName] = new List<Dictionary<string, string>>();

        tableDefinitions[tableName].Add(row);

        return tableDefinitions;
    }

    private void MergeTableDefinitions(
        Dictionary<string, List<Dictionary<string, string>>> main,
        Dictionary<string, List<Dictionary<string, string>>> toMerge)
    {
        foreach (var (key, value) in toMerge)
        {
            if (!main.ContainsKey(key))
                main[key] = new List<Dictionary<string, string>>();

            main[key].AddRange(value);
        }
    }
}