using System.Text.Json;

namespace Feedboards.Json.Sqlify.Infrastructure.JSON;

public interface IJsonAnalyzer
{
    Dictionary<string, string> AnalyzeJsonStructure(JsonElement jsonData, string prefix);
}