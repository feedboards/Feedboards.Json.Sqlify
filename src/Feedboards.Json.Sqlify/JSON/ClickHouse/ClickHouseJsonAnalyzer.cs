using System.Text.Json;
using Feedboards.Json.Sqlify.Infrastructure.JSON;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseJsonAnalyzer : IJsonAnalyzer
{
	private readonly ClickHouseObjectComparer clickHouseObjectComparer = new();
	private readonly ClickHouseTypeDetector clickHouseTypeDetector = new();

	/// <summary>
	/// Recursively analyze the structure of a JSON object to determine field types.
	/// Returns a dictionary mapping field paths to their ClickHouse data types.
	/// </summary>
	public Dictionary<string, string> AnalyzeJsonStructure(JsonElement jsonData, string prefix)
	{
		var structure = new Dictionary<string, string>();

		// Handle root-level array
		if (jsonData.ValueKind == JsonValueKind.Array && string.IsNullOrEmpty(prefix))
		{
			var arr = jsonData.EnumerateArray().ToList();

			if (arr.Count > 0)
			{
				return clickHouseObjectComparer.SumUpArrays(arr);
			}
			else
			{
				structure[prefix] = "Nullable(Array(String))";
			}

			return structure;
		}
		else if (jsonData.ValueKind == JsonValueKind.Object)
		{
			foreach (var prop in jsonData.EnumerateObject())
			{
				var safeKey = prop.Name.Replace(" ", "_");
				var fieldPath = string.IsNullOrEmpty(prefix) ? safeKey : $"{prefix}.{safeKey}";
				var value = prop.Value;

				// Handle arrays
				if (value.ValueKind == JsonValueKind.Array)
				{
					var arr = value.EnumerateArray().ToList();

					if (arr.Count > 0)
					{
						var result = clickHouseObjectComparer.SumUpArrays(arr);
						var formattedString = ClickHouseJsonUtils.FormatNestedStructure(result);

						structure[fieldPath] = clickHouseTypeDetector.DetectNullableType(value, formattedString);
					}
					else
					{
						structure[fieldPath] = clickHouseTypeDetector.DetectNullableType(value, "Array(String)");
					}

					continue;
				}
				else if (value.ValueKind == JsonValueKind.Object)
				{
					var result = AnalyzeJsonStructure(value, prefix);

					foreach (var kvp in result)
					{
						structure[kvp.Key] = kvp.Value;
					}

					continue;
				}

				// Handle simple values
				var type = clickHouseTypeDetector.DetectType(value);
				type = clickHouseTypeDetector.DetectNullableType(value, type);

				structure[fieldPath] = type;
			}
		}
		else
		{
			// Handle simple values at root level
			structure[prefix] = clickHouseTypeDetector.DetectType(jsonData);
		}

		return structure;
	}
}