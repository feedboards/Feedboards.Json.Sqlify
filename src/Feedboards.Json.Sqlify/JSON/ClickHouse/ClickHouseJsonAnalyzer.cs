using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseJsonAnalyzer
{
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
				var result = SumUpArrays(arr);

				var stringBuilder = new StringBuilder();
				foreach (var kvp in result)
				{
					stringBuilder.AppendLine($"   `{kvp.Key}` {kvp.Value},");
				}

				return result;
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

				// Handle arrays
				if (value.ValueKind == JsonValueKind.Array)
				{
					var arr = value.EnumerateArray().ToList();
					if (arr.Count > 0)
					{
						var result = SumUpArrays(arr);

						var stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Nested(");
						foreach (var kvp in result)
						{
							stringBuilder.AppendLine($"   `{kvp.Key}` {kvp.Value},");
						}
						stringBuilder.Append(")");

						structure[fieldPath] = MakeNullableIfNeeded(
							stringBuilder.ToString(), value);
					}
					else
					{
						structure[fieldPath] = MakeNullableIfNeeded("Array(String)", value);
					}
					continue;
				}

				// Handle objects
				if (value.ValueKind == JsonValueKind.Object)
				{
					var result = AnalyzeJsonStructure(value, prefix, maxDepth, currentDepth);

					foreach (var kvp in result)
					{
						structure[kvp.Key] = kvp.Value;
					}
					continue;
				}

				// Handle simple values
				var type = GetClickHouseType(value);
				type = MakeNullableIfNeeded(type, value);
				structure[fieldPath] = type;
			}
		}
		else
		{
			// Handle simple values at root level
			structure[prefix] = GetClickHouseType(jsonData);
		}

		return structure;
	}

	private string GetClickHouseType(JsonElement value)
	{
		switch (value.ValueKind)
		{
			case JsonValueKind.String:
				// Try to detect date/time types
				if (DateTime.TryParse(value.GetString(), out _))
					return "DateTime";
				return "String";
			case JsonValueKind.Number:
				if (value.TryGetInt64(out var int64))
				{
					// Determine the most appropriate integer type
					if (int64 >= -128 && int64 <= 127)
						return "Int8";
					if (int64 >= -32768 && int64 <= 32767)
						return "Int16";
					if (int64 >= -2147483648 && int64 <= 2147483647)
						return "Int32";
					return "Int64";
				}
				if (value.TryGetDouble(out var doubleValue))
				{
					// Determine the most appropriate float type
					if (doubleValue >= float.MinValue && doubleValue <= float.MaxValue)
						return "Float32";
					return "Float64";
				}
				return "String";
			case JsonValueKind.True:
			case JsonValueKind.False:
				return "UInt8";
			case JsonValueKind.Null:
				// For null values, we'll use Nullable(String) as default
				// The actual type will be determined by the context
				return "Nullable(String)";
			default:
				return null;
		}
	}

	/// <summary>
	/// Determines if a value should be treated as Nullable
	/// </summary>
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
			return string.IsNullOrEmpty(str) || str.ToLower() == "null";
		}

		// For an empty array
		if (value.ValueKind == JsonValueKind.Array)
		{
			return value.ToString() == "[]";
		}

		return false;
	}

	/// <summary>
	/// Wraps a type with Nullable if needed
	/// </summary>
	private string MakeNullableIfNeeded(string type, JsonElement value)
	{
		if (ShouldBeNullable(value))
		{
			return $"Nullable({type})";
		}
		return type;
	}

	private Dictionary<string, string> SumUpArrays(List<JsonElement> array)
	{
		var result = new Dictionary<string, string>();

		for (int i = 0; i < array.Count - 1; i++)
		{
			if (i == 0)
			{ 
				result = CompereTwoArrays(
					DetectTypeOfPropertyInArray(array[i]),
					array[i + 1]);
			}
			else
			{
				result = CompereTwoArrays(result, array[i + 1]);
			}
		}

		return result;
	}

	private Dictionary<string, string> CompereTwoArrays(
		Dictionary<string, string> firstElement, 
		JsonElement secondElement)
	{
		var allFieldsOfSecondArray = DetectTypeOfPropertyInArray(secondElement);

		//Check if any property is null
		var result = new Dictionary<string, string>();
		if (firstElement.Count > allFieldsOfSecondArray.Count)
		{
			result = SetNullable(firstElement, allFieldsOfSecondArray);
		}
		else
		{
			result = SetNullable(allFieldsOfSecondArray, firstElement);
		}

		Dictionary<string, string> SetNullable(
			Dictionary<string, string> firstElement,
			Dictionary<string, string> secondElement)
		{
			var setNullResult = new Dictionary<string, string>();

			foreach (var property in firstElement)
			{
				if (!secondElement.ContainsKey(property.Key))
				{
					setNullResult.Add(property.Key, $"Nullable({property.Value})");
				}
				else
				{
					setNullResult.Add(property.Key, property.Value);
				}
			}

			return setNullResult;
		}

		return result;
	}

	private Dictionary<string, string> DetectTypeOfPropertyInArray(JsonElement element)
	{
		var result = new Dictionary<string, string>();

		try
		{
			foreach (var property in element.EnumerateObject())
			{
				// Simple types
				var type = GetClickHouseType(property.Value);
				if (!string.IsNullOrEmpty(type))
				{
					result[property.Name] = MakeNullableIfNeeded(type, property.Value);
				}

				if (property.Value.ValueKind == JsonValueKind.Array)
				{
					var arrayType = DetectTypeOfPropertyInArray(property.Value);

					if (arrayType.Count == 1)
					{
						result[property.Name] = MakeNullableIfNeeded(
							arrayType.FirstOrDefault().Value, property.Value);
					}
					else
					{
						//TODO format properties
						var stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Nested(");
						foreach (var kvp in arrayType)
						{
							stringBuilder.AppendLine($"   `{kvp.Key}` {kvp.Value},");
						}
						stringBuilder.Append(")");

						result[property.Name] = MakeNullableIfNeeded(
							stringBuilder.ToString(), property.Value);
					}
				}

				if (property.Value.ValueKind == JsonValueKind.Object)
				{
					result[property.Name] = "Object";

					//TODO Handle nested objects 
				}
			}
		}
		catch (InvalidOperationException exc)
		{
			//TODO review this part of code
			var typesInArray = new Dictionary<string, string>();

			var index = 0;
			foreach (var property in element.EnumerateArray())
			{
				if (property.ValueKind == JsonValueKind.Object)
				{
					return DetectTypeOfPropertyInArray(property);
				}
				else
				{
					var type = GetClickHouseType(property);

					typesInArray[index.ToString()] = type;
					index++;
				}
			}

			if (typesInArray.Values.Distinct().Count() == 1)
			{
				result["Array"] = $"Array({typesInArray.FirstOrDefault().Value})";
			}
			else if (typesInArray.Values.Distinct().Count() == 0)
			{
				result["Array"] = "Array(String)";
			}
			else
			{
				var allTypes = typesInArray.Values.Distinct();
				var formattedTypes = string.Empty;

				foreach (var type in allTypes)
				{
					if (string.IsNullOrEmpty(formattedTypes))
					{
						formattedTypes = type.ToString().Trim();
					}
					else
					{
						formattedTypes += $", {type.ToString().Trim()}";
					}
				}

				result["Tuple"] = $"Tuple({formattedTypes})";
			}
		}

		return result;
	}
}