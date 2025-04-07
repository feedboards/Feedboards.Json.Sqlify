namespace Feedboards.Json.Sqlify.JSON.ClickHouse;

internal static class ClickHouseJsonUtils
{
	public static string FormatNestedStructure(Dictionary<string, string> structure)
	{
		var formattedString = "Nested(";
		foreach (var kvp in structure)
		{
			if (formattedString == "Nested(")
			{
				formattedString += $"`{kvp.Key}` {kvp.Value}";
			}
			else
			{
				formattedString += $",`{kvp.Key}` {kvp.Value}";
			}
		}
		formattedString += ")";

		return formattedString;
	}
}
