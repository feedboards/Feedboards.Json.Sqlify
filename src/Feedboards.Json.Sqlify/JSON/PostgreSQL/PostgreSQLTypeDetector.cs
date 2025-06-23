using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.PostgreSQL;

public class PostgreSQLTypeDetector
{
    public string DetectType(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => "VARCHAR",
            JsonValueKind.Number when value.TryGetInt64(out _) => "BIGINT",
            JsonValueKind.Number => "DOUBLE PRECISION",
            JsonValueKind.True or JsonValueKind.False => "BOOLEAN",
            JsonValueKind.Null => "TEXT", // Fallback for null
            _ => "JSONB"
        };
    }
}