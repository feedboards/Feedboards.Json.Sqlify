using System.Data;
using System.Globalization;
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
	public Dictionary<string, string> AnalyzeJsonStructure(JsonElement jsonData, string prefix, int maxDepth, int currentDepth)
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
				//TODO Scan all items of array
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

				// Handle arrays
				if (value.ValueKind == JsonValueKind.Array)
				{
					var arr = value.EnumerateArray().ToList();

					//TODO Scan all items of array
					if (arr.Count > 0)
					{
						// Check if array contains objects (tuple) or simple values

						if (arr[0].ValueKind == JsonValueKind.Object)
						{
							SumUpArray(arr);

							// For arrays of objects, collect all possible fields from all objects
							var allFields = new Dictionary<string, JsonElement>();
							foreach (var obj in arr)
							{
								foreach (var property in obj.EnumerateObject())
								{
									if (!allFields.ContainsKey(property.Name))
									{
										allFields[property.Name] = property.Value;
									}
								}
							}

							// Create tuple fields with nullable types
							var tupleFields = new List<string>();
							foreach (var field in allFields)
							{
								var fieldType = GetClickHouseType(field.Value);
								// Make field nullable if it doesn't appear in all objects
								if (arr.Any(obj => !obj.EnumerateObject().Any(p => p.Name == field.Key)))
								{
									fieldType = $"Nullable({fieldType})";
								}
								tupleFields.Add($"`{field.Key}` {fieldType}");
							}
							structure[fieldPath] = $"Array(Tuple({string.Join(", ", tupleFields)}))";
						}
						else
						{
							// For arrays of simple values
							var elementType = GetClickHouseType(arr[0]);
							elementType = MakeNullableIfNeeded(elementType, arr[0]);
							structure[fieldPath] = $"Array({elementType})";
						}
					}
					else
					{
						// Empty array - default to Array(String)
						structure[fieldPath] = "Array(String)"; //TODO
					}
					continue;
				}

				// Handle objects (tuples)
				if (value.ValueKind == JsonValueKind.Object)
				{
					var tupleFields = new List<string>();
					foreach (var obj in value.EnumerateObject())
					{
						var fieldType = GetClickHouseType(obj.Value);
						fieldType = MakeNullableIfNeeded(fieldType, obj.Value);
						tupleFields.Add($"`{obj.Name}` {fieldType}");
					}
					structure[fieldPath] = $"Tuple({string.Join(", ", tupleFields)})";
					continue;
				}

				// Handle simple values
				var type = GetClickHouseType(value);
				type = MakeNullableIfNeeded(type, value);
				structure[fieldPath] = type;
			}
		}
		else if (jsonData.ValueKind == JsonValueKind.Array)
		{
			// Handle nested arrays
			var arr = jsonData.EnumerateArray().ToList();
			if (arr.Count > 0)
			{
				var elementType = GetClickHouseType(arr[0]);
				structure[prefix] = $"Array({elementType})";
			}
			else
			{
				structure[prefix] = "Array(String)";
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
			case JsonValueKind.Object:
				var tupleFields = new List<string>();
				foreach (var obj in value.EnumerateObject())
				{
					var fieldType = GetClickHouseType(obj.Value);
					fieldType = MakeNullableIfNeeded(fieldType, obj.Value);
					tupleFields.Add($"`{obj.Name}` {fieldType}");
				}
				return $"Tuple({string.Join(", ", tupleFields)})";
			case JsonValueKind.Array:
				var arr = value.EnumerateArray().ToList();
				if (arr.Count > 0)
				{
					if (arr[0].ValueKind == JsonValueKind.Object)
					{
						// For arrays of objects, collect all possible fields from all objects
						var allFields = new Dictionary<string, JsonElement>();
						foreach (var obj in arr)
						{
							foreach (var prop in obj.EnumerateObject())
							{
								if (!allFields.ContainsKey(prop.Name))
								{
									allFields[prop.Name] = prop.Value;
								}
							}
						}

						// Create tuple fields with nullable types
						var listOfTupleFields = new List<string>();
						foreach (var field in allFields)
						{
							var fieldType = GetClickHouseType(field.Value);
							// Make field nullable if it doesn't appear in all objects
							if (arr.Any(obj => !obj.EnumerateObject().Any(p => p.Name == field.Key)))
							{
								fieldType = $"Nullable({fieldType})";
							}
							listOfTupleFields.Add($"`{field.Key}` {fieldType}");
						}
						return $"Array(Tuple({string.Join(", ", listOfTupleFields)}))";
					}
					else
					{
						// For arrays of simple values
						var elementType = GetClickHouseType(arr[0]);
						elementType = MakeNullableIfNeeded(elementType, arr[0]);
						return $"Array({elementType})";
					}
				}
				return "Array(String)";
			default:
				return "String";
		}
	}

	/// <summary>
	/// Determines if a value should be treated as Nullable
	/// </summary>
	private bool ShouldBeNullable(JsonElement value)
	{
		// Check if the value is null
		if (value.ValueKind == JsonValueKind.Null)
			return true;

		// For numbers, check if it's zero (which might indicate null in some contexts)
		if (value.ValueKind == JsonValueKind.Number)
		{
			if (value.TryGetInt64(out var int64) && int64 == 0)
				return true;
			if (value.TryGetDouble(out var doubleValue) && doubleValue == 0)
				return true;
		}

		// For strings, check if it's empty or "null"
		if (value.ValueKind == JsonValueKind.String)
		{
			var str = value.GetString();
			return string.IsNullOrEmpty(str) || str.ToLower() == "null";
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

	private void SumUpArray(List<JsonElement> array)
	{
		for (int i = 0; i < array.Count - 1; i++)
		{
			var firstElement = array[i];
			var secondElement = array[i + 1];
			
			var result = CompereTwoArrays(firstElement, secondElement);
		}
	}

	private Dictionary<string, string> CompereTwoArrays(
		JsonElement firstElement, JsonElement secodElement)
	{
		var allFieldsOfFirstArray = new Dictionary<string, string>();
		foreach (var property in firstElement.EnumerateObject())
		{
			if (property.Value.ValueKind == JsonValueKind.Array)
			{
				allFieldsOfFirstArray[property.Name] = "array";
			}

			if (property.Value.ValueKind == JsonValueKind.Object)
			{
				allFieldsOfFirstArray[property.Name] = "object";
			}
		}

		var allFieldsOfSecondArray = new Dictionary<string, string>();
		foreach (var property in secodElement.EnumerateObject())
		{
			if (property.Value.ValueKind == JsonValueKind.Array)
			{
				allFieldsOfSecondArray[property.Name] = "array";
			}

			if (property.Value.ValueKind == JsonValueKind.Object)
			{
				allFieldsOfSecondArray[property.Name] = "object";
			}
		}

		//Check if any property is null
		var result = new Dictionary<string, string>();
		if (allFieldsOfFirstArray.Count > allFieldsOfSecondArray.Count)
		{
			foreach (var property in allFieldsOfFirstArray)
			{
				if (!allFieldsOfSecondArray.ContainsKey(property.Key))
				{
					result.Add(property.Key, $"null {property.Value}");
				}
				else
				{
					result.Add(property.Key, property.Value);
				}
			}
		}
		else
		{
			foreach (var property in allFieldsOfSecondArray)
			{
				if (!allFieldsOfFirstArray.ContainsKey(property.Key))
				{
					result.Add(property.Key, $"null {property.Value}");
				}
				else
				{
					result.Add(property.Key, property.Value);
				}
			}
		}

		return result;
	}
}