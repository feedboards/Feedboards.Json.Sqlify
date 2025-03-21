using System.Data;
using System.Globalization;
using System.Text.Json;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseJsonAnalyzer
{
	/// <summary>
	/// Recursively analyze the structure of a JSON object to determine field types.
	/// Returns a dictionary mapping field paths to their ClickHouse data types.
	/// </summary>
	public Dictionary<string, string> AnalyzeJsonStructure(JsonElement jsonData, string prefix, int maxDepth, int currentDepth)
	{
		var structure = new Dictionary<string, string>();

		if (maxDepth > 0 && currentDepth >= maxDepth)
		{
			throw new NestedStructureLimitException(maxDepth, currentDepth);
		}

		// Handle root-level array
		if (jsonData.ValueKind == JsonValueKind.Array && string.IsNullOrEmpty(prefix))
		{
			var arr = jsonData.EnumerateArray().ToList();
			if (arr.Count > 0)
			{
				// Take the first item as a sample for structure
				return AnalyzeJsonStructure(arr[0], prefix, maxDepth, currentDepth);
			}
			return structure;
		}

		if (jsonData.ValueKind == JsonValueKind.Object)
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

				if (value.ValueKind == JsonValueKind.Object)
				{
					var nestedStructure = AnalyzeJsonStructure(value, "", maxDepth, currentDepth + 1);
					if (nestedStructure.Count > 0)
					{
						var nestedFields = new List<string>();
						foreach (var kv in nestedStructure.OrderBy(k => k.Key))
						{
							nestedFields.Add($"`{kv.Key}` {kv.Value}");
						}
						structure[fieldPath] = $"Nested(\n        {string.Join(",\n        ", nestedFields)}\n    )";
					}
					else
					{
						structure[fieldPath] = "String";
					}
				}
				else if (value.ValueKind == JsonValueKind.Array)
				{
					var arr = value.EnumerateArray().ToList();
					if (arr.Count > 0)
					{
						if (arr.All(item => item.ValueKind == JsonValueKind.Object))
						{
							var sample = arr.First();
							var nestedStructure = AnalyzeJsonStructure(sample, "", maxDepth, currentDepth + 1);
							if (nestedStructure.Count > 0)
							{
								var nestedFields = new List<string>();
								foreach (var kv in nestedStructure.OrderBy(k => k.Key))
								{
									// For nested arrays, handle IDs consistently
									var fieldType = kv.Key.EndsWith("_id") || kv.Key == "id" ? "UInt64" : kv.Value;
									nestedFields.Add($"`{kv.Key}` {fieldType}");
								}
								structure[fieldPath] = $"Nested(\n        {string.Join(",\n        ", nestedFields)}\n    )";
							}
							else
							{
								structure[fieldPath] = "Array(String)";
							}
						}
						else
						{
							var types = arr.Where(item => item.ValueKind != JsonValueKind.Null)
										 .Select(item => DetectType(item))
										 .ToList();

							if (types.Count == 0)
							{
								structure[fieldPath] = "Array(String)";
							}
							else if (types.All(t => t == "UInt64"))
							{
								structure[fieldPath] = "Array(UInt64)";
							}
							else if (types.All(t => t.StartsWith("UInt")))
							{
								structure[fieldPath] = "Array(UInt64)";
							}
							else if (types.All(t => t.StartsWith("Int")))
							{
								structure[fieldPath] = "Array(Int64)";
							}
							else if (types.Any(t => t == "Float64"))
							{
								structure[fieldPath] = "Array(Float64)";
							}
							else if (types.All(t => t == "DateTime64(3)"))
							{
								structure[fieldPath] = "Array(DateTime64(3))";
							}
							else
							{
								structure[fieldPath] = "Array(String)";
							}
						}
					}
					else
					{
						structure[fieldPath] = "Array(String)";
					}
				}
				else
				{
					structure[fieldPath] = DetectType(value);
				}
			}
		}

		return structure;
	}

	private static string DetectType(JsonElement value)
	{
		if (value.ValueKind == JsonValueKind.Null)
		{
			return "Nullable(String)";
		}
		else if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
		{
			return "UInt8";
		}
		else if (value.ValueKind == JsonValueKind.Number)
		{
			if (value.TryGetInt64(out long intVal))
			{
				if (intVal >= 0)
				{
					return "UInt64"; // Use UInt64 consistently for all positive integers
				}
				else
				{
					return "Int64"; // Use Int64 consistently for negative integers
				}
			}
			else
			{
				return "Float64";
			}
		}
		else if (value.ValueKind == JsonValueKind.String)
		{
			var str = value.GetString();
			if (!string.IsNullOrEmpty(str))
			{
				if (str.Length >= 19 && str[4] == '-' && str[7] == '-' && (str[10] == 'T' || str[10] == ' ') && str[13] == ':' && str[16] == ':')
				{
					if (DateTime.TryParse(str, out _))
					{
						return "DateTime64(3)";
					}
				}
				else if (str.Length == 10 && str[4] == '-' && str[7] == '-')
				{
					if (DateTime.TryParseExact(str, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
					{
						return "Date";
					}
				}
			}
			return "String";
		}
		else
		{
			return "String";
		}
	}
}