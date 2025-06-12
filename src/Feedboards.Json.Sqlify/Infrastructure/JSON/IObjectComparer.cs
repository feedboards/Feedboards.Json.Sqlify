using System.Text.Json;

namespace Feedboards.Json.Sqlify.Infrastructure.JSON;

public interface IObjectComparer
{
    Dictionary<string, string> SumUpArrays(List<JsonElement> array);
}