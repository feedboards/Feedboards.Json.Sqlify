using System.Text.Json;

namespace Feedboards.Json.Sqlify.Infrastructure.JSON;

public interface ITypeDetector
{
    string DetectType(JsonElement value);
    string DetectNullableType(JsonElement value, string detectedType);
}