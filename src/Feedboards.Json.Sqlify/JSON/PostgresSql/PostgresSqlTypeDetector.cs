using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgresSql;

public class PostgresSqlTypeDetector
{
    private readonly string notNullDefinition = "NOT NULL";
    
    public string? DetectType(JsonElement value)
    {
        var result = value.ValueKind switch
        {
            JsonValueKind.String => "VARCHAR",
            JsonValueKind.Number when value.TryGetInt64(out _) => "BIGINT",
            JsonValueKind.Number => "DOUBLE PRECISION",
            JsonValueKind.True or JsonValueKind.False => "BOOLEAN",
            JsonValueKind.Null => "TEXT", // Fallback for null
            _ => null
        };

        // Set value as not nullable by default
        result += $" {notNullDefinition}";

        return result;
    }

    public string DetectNullableType(JsonElement value, string? type)
    {
        //TODO update exception
        ArgumentException.ThrowIfNullOrEmpty(type);
        
        if (!type.Contains(notNullDefinition))
        {
            return type;
        }
        else if (ShouldBeNullable(value))
        {
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