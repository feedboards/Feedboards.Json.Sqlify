using System.Data;
using System.Globalization;
using System.Text.Json;

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

		if (currentDepth >= maxDepth)
		{
			return structure;
		}

		if (jsonData.ValueKind == JsonValueKind.Object)
		{
			foreach (var prop in jsonData.EnumerateObject())
			{
				// Replace spaces in key names with underscores.
				var safeKey = prop.Name.Replace(" ", "_");
				var fieldPath = string.IsNullOrEmpty(prefix) ? safeKey : $"{prefix}.{safeKey}";
				var value = prop.Value;

				if (value.ValueKind == JsonValueKind.Object)
				{
					// Handle nested object – create a Nested type.
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
				else if (value.ValueKind == JsonValueKind.Array && value.GetArrayLength() > 0)
				{
					var arr = value.EnumerateArray().ToList();

					if (arr.All(item => item.ValueKind == JsonValueKind.Object))
					{
						// Create a nested structure for arrays of objects.
						var sample = arr.First();
						var nestedStructure = AnalyzeJsonStructure(sample, "", maxDepth, currentDepth + 1);

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
							structure[fieldPath] = "Array(String)";
						}
					}
					else if (arr.All(item =>
						item.ValueKind == JsonValueKind.String ||
						item.ValueKind == JsonValueKind.Number ||
						item.ValueKind == JsonValueKind.True ||
						item.ValueKind == JsonValueKind.False ||
						item.ValueKind == JsonValueKind.Null))
					{
						var types = arr.Where(item => item.ValueKind != JsonValueKind.Null)
									 .Select(item => DetectType(item))
									 .ToList();
						if (types.Count == 0)
						{
							structure[fieldPath] = "Array(String)";
						}
						else
						{
							if (types.All(t => t.StartsWith("UInt")))
							{
								var maxType = types.OrderBy(t =>
								{
									var numStr = new string(t.Skip(4).TakeWhile(Char.IsDigit).ToArray());
									return int.TryParse(numStr, out int n) ? n : 0;
								}).Last();

								structure[fieldPath] = $"Array({maxType})";
							}
							else if (types.All(t => t.StartsWith("Int")))
							{
								var maxType = types.OrderBy(t =>
								{
									var numStr = new string(t.Skip(3).TakeWhile(Char.IsDigit).ToArray());
									return int.TryParse(numStr, out int n) ? n : 0;
								}).Last();

								structure[fieldPath] = $"Array({maxType})";
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
			// Try to treat the number as an integer first.
			if (value.TryGetInt64(out long intVal))
			{
				if (intVal >= 0)
				{
					if (intVal < (1L << 8))
					{
						return "UInt8";
					}
					else if (intVal < (1L << 16))
					{
						return "UInt16";
					}
					else if (intVal < (1L << 32))
					{
						return "UInt32";
					}
					else
					{
						return "UInt64";
					}
				}
				else
				{
					if (intVal >= -(1L << 7) && intVal < (1L << 7))
					{
						return "Int8";
					}
					else if (intVal >= -(1L << 15) && intVal < (1L << 15))
					{
						return "Int16";
					}
					else if (intVal >= -(1L << 31) && intVal < (1L << 31))
					{
						return "Int32";
					}
					else
					{
						return "Int64";
					}
				}
			}
			else
			{
				// If not an integer, use float type.
				return "Float64";
			}
		}
		else if (value.ValueKind == JsonValueKind.String)
		{
			var str = value.GetString();

			if (!string.IsNullOrEmpty(str))
			{
				// Check for date in the format yyyy-MM-dd
				if (str.Length == 10 && str[4] == '-' && str[7] == '-')
				{
					if (DateTime.TryParseExact(str, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
					{
						return "Date";
					}
				}
				// Check for datetime in a typical ISO 8601 format (e.g. yyyy-MM-ddTHH:mm:ss)
				if (str.Length >= 19 && str[4] == '-' && str[7] == '-' && str[10] == 'T' && str[13] == ':' && str[16] == ':')
				{
					if (DateTime.TryParse(str, out _))
					{
						return "DateTime64(3)";
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