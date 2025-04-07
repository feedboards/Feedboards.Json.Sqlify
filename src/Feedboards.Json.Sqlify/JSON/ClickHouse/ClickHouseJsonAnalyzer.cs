using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;
using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseJsonAnalyzer
{
	private readonly ClickHouseObjectComparer clickHouseObjectComparer; 
	private readonly ClickHouseTypeDetector clickHouseTypeDetector;

	public ClickHouseJsonAnalyzer()
	{
		this.clickHouseObjectComparer = new ClickHouseObjectComparer();
		this.clickHouseTypeDetector = new ClickHouseTypeDetector();
	}

	/// <summary>
	/// Recursively analyze the structure of a JSON object to determine field types.
	/// Returns a dictionary mapping field paths to their ClickHouse data types.
	/// </summary>
	public Dictionary<string, string> AnalyzeJsonStructure(
		JsonElement jsonData,
		string prefix,
		int maxDepth,
		int currentDepth)
	{
		var structure = new Dictionary<string, string>();

		if (maxDepth > 0 && currentDepth >= maxDepth)
		{
			throw new NestedStructureLimitException(
				actualDepth: currentDepth,
				maxAllowedDepth: maxDepth);
		}

		// Handle root-level array
		if (jsonData.ValueKind == JsonValueKind.Array && string.IsNullOrEmpty(prefix))
		{
			var arr = jsonData.EnumerateArray().ToList();
			if (arr.Count > 0)
			{
				return clickHouseObjectComparer.SumUpArrays(arr);
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

				// Handle ID fields consistently
				if (safeKey.EndsWith("_id") || safeKey == "id")
				{
					structure[fieldPath] = "UInt64";
					continue;
				}

				// Handle arrays
				if (value.ValueKind == JsonValueKind.Array)
				{
					var arr = value.EnumerateArray().ToList();
					if (arr.Count > 0)
					{
						var result = clickHouseObjectComparer.SumUpArrays(arr);
						var formattedString = ClickHouseJsonUtils.FormatNestedStructure(result);

						structure[fieldPath] = clickHouseTypeDetector.MakeNullableIfNeeded(formattedString, value);
					}
					else
					{
						structure[fieldPath] = clickHouseTypeDetector.MakeNullableIfNeeded("Array(String)", value);
					}
					continue;
				}
				else if (value.ValueKind == JsonValueKind.Object)
				{
					var result = AnalyzeJsonStructure(value, prefix, maxDepth, currentDepth);

					foreach (var kvp in result)
					{
						structure[kvp.Key] = kvp.Value;
					}
					continue;
				}

				// Handle simple values
				var type = clickHouseTypeDetector.GetClickHouseType(value);
				type = clickHouseTypeDetector.MakeNullableIfNeeded(type, value);
				structure[fieldPath] = type;
			}
		}
		else
		{
			// Handle simple values at root level
			structure[prefix] = clickHouseTypeDetector.GetClickHouseType(jsonData);
		}

		return structure;
	}
}