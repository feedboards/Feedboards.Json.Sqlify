using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal class ClickHouseObjectComparer
{
	private readonly ClickHouseTypeDetector clickHouseTypeDetector;

	public ClickHouseObjectComparer()
	{
		this.clickHouseTypeDetector = new ClickHouseTypeDetector();
	}

	public Dictionary<string, string> SumUpArrays(List<JsonElement> array)
	{
		var result = new Dictionary<string, string>();

		if (array.Count == 1)
		{
			result = DetectTypeOfPropertyInArray(array[0]);

			return result;
		}

		for (int i = 0; i < array.Count - 1; i++)
		{
			if (i == 0)
			{
				result = ComparerTwoArrays(
					DetectTypeOfPropertyInArray(array[i]),
					array[i + 1]);
			}
			else
			{
				result = ComparerTwoArrays(result, array[i + 1]);
			}
		}

		return result;
	}

	private Dictionary<string, string> TranslateStringObjectToDictionary(string stringObject)
	{
		var preparedObject = stringObject.Substring(7);
		var input = preparedObject.Remove(preparedObject.Length - 1);

		var parts = SplitOnTopLevelCommas(input);

		var result = new Dictionary<string, string>();
		var pattern = @"^\s*(`[^`]+`)\s+(.+?)\s*$";

		foreach (string part in parts)
		{
			var trimmedPart = part.Trim();
			var match = Regex.Match(trimmedPart, pattern);

			if (match.Success)
			{
				var transfer = match.Groups[1].Value.Substring(1);
				var key = transfer.Remove(transfer.Length - 1);
				var value = match.Groups[2].Value;

				result[key] = value;
			}
		}

		static List<string> SplitOnTopLevelCommas(string s)
		{
			var resultList = new List<string>();
			var sb = new StringBuilder();
			var parenCount = 0;

			foreach (char c in s)
			{
				if (c == ',' && parenCount == 0)
				{
					resultList.Add(sb.ToString());
					sb.Clear();
				}
				else
				{
					if (c == '(')
					{
						parenCount++;
					}
					else if (c == ')')
					{
						parenCount--;
					}

					sb.Append(c);
				}
			}

			if (sb.Length > 0)
			{
				resultList.Add(sb.ToString());
			}

			return resultList;
		}

		return result;
	}

	private Dictionary<string, string> ComparerTwoNestedObjectsFromTheString(
		string firstObject,
		string secondObject)
	{
		var firstObjectInDict = TranslateStringObjectToDictionary(firstObject);
		var secondObjectInDict = TranslateStringObjectToDictionary(secondObject);

		var result = new Dictionary<string, string>();

		if (firstObjectInDict.Count > secondObjectInDict.Count)
		{
			result = SetNullable(firstObjectInDict, secondObjectInDict);
		}
		else
		{
			result = SetNullable(secondObjectInDict, firstObjectInDict);
		}

		return result;
	}

	private Dictionary<string, string> ComparerTwoArrays(
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

		return result;
	}

	private Dictionary<string, string> SetNullable(
		Dictionary<string, string> firstElement,
		Dictionary<string, string> secondElement)
	{
		var setNullResult = new Dictionary<string, string>();

		foreach (var property in firstElement)
		{
			//In this case we need to take the same property from the second object if there exists this property.
			if (property.Value == "Array(String)" &&
			    secondElement.TryGetValue(property.Key, out var value) &&
			    value.StartsWith("Nested(")) // We use `Array(String)` because it is default value for an empty array
			{
				setNullResult[property.Key] = value;
			}
			else if (
				property.Value.StartsWith("Nested(") &&
				secondElement.ContainsKey(property.Key))
			{
				var res = ComparerTwoNestedObjectsFromTheString(
					property.Value,
					secondElement[property.Key]);

				setNullResult[property.Key] = ClickHouseJsonUtils.FormatNestedStructure(res);
			}
			else if (!secondElement.ContainsKey(property.Key))
			{
				if (
					property.Value.StartsWith("Array(") ||
					property.Value.StartsWith("Nullable("))
				{
					setNullResult[property.Key] = property.Value;
				}
				else
				{
					setNullResult[property.Key] = $"Nullable({property.Value})";
				}
			}
			else
			{
				setNullResult[property.Key] = property.Value;
			}
		}

		return setNullResult;
	}

	private Dictionary<string, string> DetectTypeOfPropertyInArray(JsonElement element)
	{
		var result = new Dictionary<string, string>();
		
		if (element.ValueKind == JsonValueKind.Object)
		{
			foreach (var property in element.EnumerateObject())
			{
				// Simple types
				var type = clickHouseTypeDetector.GetClickHouseType(property.Value);
				if (!string.IsNullOrEmpty(type))
				{
					result[property.Name] = clickHouseTypeDetector.MakeNullableIfNeeded(type, property.Value);
				}
				else if (property.Value.ValueKind == JsonValueKind.Array)
				{
					var arrayType = DetectTypeOfPropertyInArray(property.Value);

					if (arrayType.Count == 1 &&
						arrayType.FirstOrDefault().Key == "Feedboards.Json.Sqlify.Array" ||
						arrayType.FirstOrDefault().Key == "Feedboards.Json.Sqlify.Tuple")
					{
						result[property.Name] = clickHouseTypeDetector.MakeNullableIfNeeded(
							arrayType.FirstOrDefault().Value, property.Value);
					}
					else
					{
						var formattedString = ClickHouseJsonUtils.FormatNestedStructure(arrayType);

						result[property.Name] = clickHouseTypeDetector.MakeNullableIfNeeded(formattedString, property.Value);
					}
				}
				else if (property.Value.ValueKind == JsonValueKind.Object)
				{
					var propertiesOfObject = DetectTypeOfPropertyInArray(property.Value);
					var formattedString = ClickHouseJsonUtils.FormatNestedStructure(propertiesOfObject);

					result[property.Name] = clickHouseTypeDetector.MakeNullableIfNeeded(formattedString, property.Value);
				}
			}
		}
		else
		{
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
					var type = clickHouseTypeDetector.GetClickHouseType(property);

					typesInArray[index.ToString()] = type;
					index++;
				}
			}

			if (typesInArray.Values.Distinct().Count() == 1)
			{
				result["Feedboards.Json.Sqlify.Array"] = $"Array({typesInArray.FirstOrDefault().Value})";
			}
			else if (typesInArray.Values.Distinct().Count() == 0)
			{
				result["Feedboards.Json.Sqlify.Array"] = clickHouseTypeDetector.MakeNullableIfNeeded("Array(String)", element);
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

				result["Feedboards.Json.Sqlify.Tuple"] = $"Tuple({formattedTypes})";
			}
		}
		
		return result;
	}
}
