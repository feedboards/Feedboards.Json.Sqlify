using System.Text.Json;
using Feedboards.Json.Sqlify.Extensions;

namespace Feedboards.Json.Sqlify.JSON.PostgresSql;

internal class PostgresSqlObjectComparer
{
    private readonly PostgresSqlTypeDetector typeDetector = new();
    
    public Dictionary<string, Dictionary<string, string>> SumUpArrays(List<JsonElement> array, string tableName)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();

        if (array.Count == 1)
        {
            return DetectStructureFromObject(array[0], tableName);
        }

        for (int i = 0; i < array.Count - 1; i++)
        {
            if (i == 0)
            {
                result = CompareTwoStructures(
                    DetectStructureFromObject(array[i], tableName),
                    array[i + 1],
                    tableName);
            }
            else
            {
                result = CompareTwoStructures(result, array[i + 1], tableName);
            }
        }
        
        return result;
    }
    private Dictionary<string, Dictionary<string, string>> DetectStructureFromObject(
        JsonElement obj,
        string tableName,
        string? fk = null)
    {
        var structure = new Dictionary<string, Dictionary<string, string>>();

        if (!structure.ContainsKey(tableName))
        {
            structure[tableName] = new Dictionary<string, string>();
        }

        if (fk != null)
        {
            structure[tableName][$"{fk}_id"] = $"INTEGER REFERENCES {fk}(id)";
        }
        
        if (obj.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in obj.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    //TODO
                    
                    // if (prop.Value.EnumerateArray().Any() && prop.Value.EnumerateArray().First().ValueKind == JsonValueKind.Object)
                    // {
                    //     var nestedTable = $"{tableName}_{prop.Name}"; //TODO test
                    //     var nested = SumUpArrays(prop.Value.EnumerateArray().ToList(), nestedTable);
                    //
                    //     foreach (var col in nested[nestedTable].Keys.ToList())
                    //     {
                    //         nested[nestedTable][col] = MarkNullable(nested[nestedTable][col]);
                    //     }
                    //
                    //     nested[nestedTable][$"{tableName}_id"] = $"INTEGER REFERENCES {tableName}(id)";
                    //     structure.MergeNestedDictionaries(nested);
                    // }
                    // else
                    // {
                    //     var elementType = typeDetector.DetectType(prop.Value.EnumerateArray().FirstOrDefault());
                    //     structure[tableName][prop.Name] = $"TEXT[]"; // Primitive array fallback
                    // }
                }
                else if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    var nestedTable = $"{tableName}_{prop.Name}";
                    var nested = DetectStructureFromObject(
                        prop.Value,
                        nestedTable,
                        "");
                    
                    //TODO test it because here we don't know about "id"
                    nested[nestedTable][$"{tableName}_id"] = $"INTEGER REFERENCES {tableName}(id)";
                    
                    structure.MergeNestedDictionaries(nested);
                }
                else
                {
                    var type = typeDetector.DetectType(prop.Value);
                    structure[tableName][prop.Name] = typeDetector.DetectNullableType(prop.Value, type);
                }
            }
        }
        else
        {
            //Array
            //TODO
        }

        return structure;
    }

    private Dictionary<string, Dictionary<string, string>> CompareTwoStructures(
        Dictionary<string, Dictionary<string, string>> first,
        JsonElement secondElement,
        string tableName)
    {
        var second = DetectStructureFromObject(secondElement, tableName);
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var (table, columns) in first)
        {
            if (!result.ContainsKey(table))
                result[table] = new();

            foreach (var (col, type) in columns)
            {
                if (second.TryGetValue(table, out var secCols) && secCols.ContainsKey(col))
                {
                    result[table][col] = type;
                }
                else
                {
                    result[table][col] = MarkNullable(type);
                }
            }
        }

        foreach (var (table, columns) in second)
        {
            if (!result.ContainsKey(table))
                result[table] = new();

            foreach (var (col, type) in columns)
            {
                if (!result[table].ContainsKey(col))
                {
                    result[table][col] = MarkNullable(type);
                }
            }
        }

        return result;
    }

    private static string MarkNullable(string type)
    {
        return type.Contains("NOT NULL") ? type.Replace("NOT NULL", "").Trim() : type;
    }
}