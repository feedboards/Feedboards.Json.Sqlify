using Feedboards.Json.Sqlify.SQL.ClickHouse.Interfaces;

namespace Feedboards.Json.Sqlify.SQL.ClickHouse;

internal class ClickHouseSQLBuilder : IClickHouseSQLBuilder
{
	public string GenerateClickHouseSchema(Dictionary<string, string> structure, string tableName)
	{
		var schemaLines = new List<string>();

		foreach (var kvp in structure.OrderBy(kvp => kvp.Key))
		{
			string fieldName = kvp.Key;
			string fieldType = kvp.Value;

			// Only process top-level fields
			if (!fieldName.Contains("."))
			{
				// Format nested structures if needed
				if (fieldType.StartsWith("Nested("))
				{
					string formattedType = FormatNestedStructure(fieldType);
					schemaLines.Add($"    `{fieldName}` {formattedType}");
				}
				else
				{
					schemaLines.Add($"    `{fieldName}` {fieldType}");
				}
			}
		}

		var schema = string.Join(",\n", schemaLines);

		var craeteTable = $@"
CREATE TABLE IF NOT EXISTS {tableName} (
{schema}
) ENGINE = MergeTree()
ORDER BY tuple();
";

		return craeteTable;
	}

	private string FormatNestedStructure(string fieldType, int indentLevel = 2)
	{
		if (!fieldType.StartsWith("Nested("))
		{
			return fieldType;
		}

		var context = fieldType.Substring(7, fieldType.Length - 8).Trim();
		var fields = new List<string>();
		var currentField = "";
		var nestedLevel = 0;

		// Parse the fields while respecting nested structures
		foreach (char contextChar in context)
		{
			if (contextChar == '(' && currentField.Contains("Nested"))
			{
				nestedLevel++;
			}
			else if (contextChar == ')')
			{
				nestedLevel--;
			}

			if (contextChar == ',' && nestedLevel == 0)
			{
				fields.Add(currentField.Trim());
				currentField = "";
			}
			else
			{
				currentField += contextChar;
			}
		}

		if (!string.IsNullOrEmpty(currentField))
		{
			fields.Add(currentField.Trim());
		}

		//Format each field
		var formattedFields = new List<string>();
		var baseIndent = new string(' ', indentLevel * 4);

		foreach (var field in fields)
		{
			if (field.Contains("Nested("))
			{
				//Recursively format nested structures
				var index = field.IndexOf("Nested(");

				var fieldName = field.Substring(0, index).Trim();
				var nestedContent = field.Substring(index);
				var formattedNested = FormatNestedStructure(nestedContent, indentLevel + 1);

				formattedFields.Add($"{fieldName}{formattedNested}");
			}
			else
			{
				formattedFields.Add(field); //.Trim()
			}
		}

		//Join fields with proper formatting
		var fieldsString = string.Join($",\n{baseIndent}", formattedFields);

		var reducedIndent = baseIndent.Length >= 4 ? baseIndent.Substring(0, baseIndent.Length - 4) : "";
		return $"Nested(\n{baseIndent}{fieldsString}\n{reducedIndent})";
	}
}
