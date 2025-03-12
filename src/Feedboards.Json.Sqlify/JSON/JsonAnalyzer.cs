using System.Data;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Feedboards.Json.Sqlify.JSON;

internal class JsonAnalyzer
{
	/// <summary>
	/// Recursively analyze the structure of a JSON object to determine field types.
	/// Returns a dictionary mapping field paths to their ClickHouse data types.
	/// </summary>
	public Dictionary<string, string> AnalyzeJsonStructure(
		JToken token, 
		string prefix = "", 
		int maxDepth = 10, 
		int currentDepth = 0)
	{
		var structure = new Dictionary<string, string>();

		if (currentDepth >= maxDepth)
		{
			return structure;
		}

		if (token.Type == JTokenType.Object)
		{
			foreach (var property in ((JObject)token).Properties())
			{
				var safeKey = property.Name.Replace(" ", "_");
				var fieldPath = string.IsNullOrEmpty(prefix) ? safeKey : $"{prefix}.{safeKey}";
				var value = property.Value;

				switch (value.Type)
				{
					case JTokenType.Object:
					{
						var nestedStructure = AnalyzeJsonStructure(
							value,
							"",
							maxDepth,
							currentDepth + 1);

						if (nestedStructure.Any())
						{
							var nestedFields = nestedStructure
								.OrderBy(n => n.Key)
								.Select(n => $"`{n.Key}` {n.Value}");

							structure[fieldPath] = $"Nested(\n        {string.Join(",", nestedFields)}\n    )";
						}
						else
						{
							structure[fieldPath] = "String";
						}

						break;
					}

					case JTokenType.Array:
					{
						if (!value.HasValues)
						{
							break;
						}

						var array = (JArray)value;

						if (array.All(item => item.Type == JTokenType.Object))
						{
							var nestedStructure = AnalyzeJsonStructure(
								array.First,
								"",
								maxDepth,
								currentDepth + 1);

							if (nestedStructure.Any())
							{
								var nestedFields = nestedStructure
									.OrderBy(n => n.Key)
									.Select(n => $"`{n.Key}` {n.Value}");
								structure[fieldPath] = $"Nested(\n        {string.Join(",", nestedFields)}\n    )";
							}
							else
							{
								structure[fieldPath] = "Array(String)";
							}
						}
						else if (array.All(item =>
									item.Type == JTokenType.String ||
									item.Type == JTokenType.Integer ||
									item.Type == JTokenType.Float ||
									item.Type == JTokenType.Boolean ||
									item.Type == JTokenType.Null))
						{
							var types = array
								.Where(item => item.Type != JTokenType.Null)
								.Select(item => DetectType(item))
								.ToList();

							if (!types.Any())
							{
								structure[fieldPath] = "Array(String)";
							}
							else if (types.All(t => t.StartsWith("UInt")))
							{
								string maxType = types.OrderBy(t =>
								{
									var numeric = 0;
									int.TryParse(t.Substring(4), out numeric);

									return numeric;
								}).Last();
								structure[fieldPath] = $"Array({maxType})";
							}
							else if (types.All(t => t.StartsWith("Int")))
							{
								string maxType = types.OrderBy(t =>
								{
									var numeric = 0;
									int.TryParse(t.Substring(3), out numeric);

									return numeric;
								}).Last();
								structure[fieldPath] = $"Array({maxType})";
							}
							else if (types.Any(t => t == "Float64"))
							{
								structure[fieldPath] = "Array(Float64)";
							}
							else
							{
								structure[fieldPath] = "Array(String)";
							}
						}
						else
						{
							structure[fieldPath] = "Array(String)";
						}

						break;
					}

					default:
						structure[fieldPath] = DetectType(value);
						break;
				}	
			}
		}

		return structure;
	}

	private string DetectType(JToken token)
	{
		switch (token.Type)
		{
			case JTokenType.Null:
				return "Nullable(String)";

			case JTokenType.Boolean:
				return "UInt8";

			case JTokenType.Integer:
				var value = token.Value<long>();

				if (value >= 0)
				{
					if (value < 256)
					{
						return "UInt8";
					}
					else if (value < 65536)
					{
						return "UInt16";
					}
					else if (value < (long)Math.Pow(2, 32))
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
					if (value >= -128 && value < 128)
					{
						return "Int8";
					}
					else if (value >= -32768 && value < 32768)
					{
						return "Int16";
					}
					else if (value >= -2147483648 && value < 2147483648)
					{
						return "Int32";
					}
					else
					{
						return "Int64";
					}
				}

			case JTokenType.Float:
				return "Float64";

			case JTokenType.String:
				var str = token.Value<string>();

				if (str.Length == 10 && str[4] == '-' && str[7] == '-')
				{
					if (DateTime.TryParseExact(
						str.Substring(0, 19), 
						"yyyy-MM-ddTHH:mm:ss", 
						CultureInfo.InvariantCulture, 
						DateTimeStyles.None, 
						out DateTime _))
					{
						return "DateTime64(3)";
					}
				}

				return "String";

			default:
				return "string";
		}
	}
}