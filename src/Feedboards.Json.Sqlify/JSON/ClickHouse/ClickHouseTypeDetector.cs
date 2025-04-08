using System.Text.Json;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseTypeDetector
{
	public string GetClickHouseType(JsonElement value)
	{
		switch (value.ValueKind)
		{
			case JsonValueKind.String:
				if (DateTime.TryParse(value.GetString(), out _))
				{
					return "String";
				}

				return "String";
			case JsonValueKind.Number:
				if (value.TryGetInt64(out var int64))
				{
					if (int64 >= -128 && int64 <= 127)
					{
						return "Int8";
					}

					if (int64 >= -32768 && int64 <= 32767)
					{
						return "Int16";
					}

					if (int64 >= -2147483648 && int64 <= 2147483647)
					{
						return "Int32";
					}

					return "Int64";
				}
				if (value.TryGetDouble(out var doubleValue))
				{
					if (doubleValue >= float.MinValue && doubleValue <= float.MaxValue)
					{
						return "Float32";
					}

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
	/// Wraps a type with Nullable if needed
	/// </summary>
	public string MakeNullableIfNeeded(string type, JsonElement value)
	{
		if (type.Contains("Nullable("))
		{
			return type;
		}
		if (ShouldBeNullable(value))
		{
			return $"Nullable({type})";
		}
		return type;
	}

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
			return false;
		}

		return false;
	}
}
