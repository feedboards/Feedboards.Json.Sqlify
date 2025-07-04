using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgresSql;

public class PostgresSqlTypeDetector
{
    private readonly string notNullDeclaration = "NOT NULL";
    //TODO add this from PGSQL 10+ version "INTEGER GENERATED ALWAYS AS IDENTITY"
    private readonly string primaryKeyDeclaration = "PRIMARY KEY";
    
    public string? DetectType(JsonElement value, bool? isPrimaryKey = null)
    {
        //TODO add this from PGSQL 10+ version "INTEGER GENERATED ALWAYS AS IDENTITY"
        //For now can be SERIAL for tests
        if (isPrimaryKey == true)
        {
            return $"SERIAL {primaryKeyDeclaration}";
        }
        
        var result = value.ValueKind switch
        {
            JsonValueKind.String => "VARCHAR",
            JsonValueKind.Number when value.TryGetInt64(out _) => "BIGINT",
            JsonValueKind.Number => "DOUBLE PRECISION",
            JsonValueKind.True or JsonValueKind.False => "BOOLEAN",
            JsonValueKind.Null => "TEXT", // Fallback for null
            _ => null
        };
        
        return result += $" {notNullDeclaration}";
    }

    public string DetectNullableType(JsonElement value, string? type)
    {
        //TODO update exception
        ArgumentException.ThrowIfNullOrEmpty(type);
        
        if (!type.Contains(notNullDeclaration))
        {
            return type;
        }
        else if (ShouldBeNullable(value))
        {
            return type.Replace(notNullDeclaration, "", StringComparison.OrdinalIgnoreCase).Trim();
        }
        
        return type;
    }
    
    private bool ShouldBeNullable(JsonElement value)
    {
        // Check if the value is null
        if (value.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        // For numbers, check if it's zero (which might indicate null in some contexts)
        if (value.ValueKind == JsonValueKind.Number)
        {
            if (value.TryGetInt64(out var int64) && int64 == 0)
            {
                return true;
            }
            if (value.TryGetDouble(out var doubleValue) && doubleValue == 0)
            {
                return true;
            }
        }

        // For strings, check if it's empty or "null"
        if (value.ValueKind == JsonValueKind.String)
        {
            var str = value.GetString();
            return string.IsNullOrEmpty(str);
        }

        return false;
    }
}