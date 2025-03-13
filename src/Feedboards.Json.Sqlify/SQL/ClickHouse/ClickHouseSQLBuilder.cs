namespace Feedboards.Json.Sqlify.SQL.ClickHouse;

internal class ClickHouseSQLBuilder
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
		return $"CREATE TABLE IF NOT EXISTS {tableName} (\n{schema}\n) ENGINE = MergeTree()\nORDER BY tuple();";
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
		var inQuotes = false;
		var parenthesesStack = 0;

		// Parse the fields while respecting nested structures and quoted identifiers
		foreach (char c in context)
		{
			if (c == '`')
			{
				inQuotes = !inQuotes;
				currentField += c;
				continue;
			}

			if (!inQuotes)
			{
				if (c == '(')
				{
					if (currentField.TrimEnd().EndsWith("Nested"))
					{
						nestedLevel++;
					}
					parenthesesStack++;
					currentField += c;
				}
				else if (c == ')')
				{
					parenthesesStack--;
					if (nestedLevel > 0 && parenthesesStack == 0)
					{
						nestedLevel--;
					}
					currentField += c;
				}
				else if (c == ',' && nestedLevel == 0 && parenthesesStack == 0)
				{
					fields.Add(currentField.Trim());
					currentField = "";
				}
				else
				{
					currentField += c;
				}
			}
			else
			{
				currentField += c;
			}
		}

		if (!string.IsNullOrEmpty(currentField))
		{
			fields.Add(currentField.Trim());
		}

		// Format each field
		var formattedFields = new List<string>();
		var baseIndent = new string(' ', indentLevel * 4);

		foreach (var field in fields)
		{
			if (field.Contains("Nested("))
			{
				// Recursively format nested structures
				var parts = field.Split(new[] { "Nested(" }, 2, StringSplitOptions.None);
				var fieldName = parts[0].Trim();
				var nestedContent = "Nested(" + parts[1];
				
				// Add space between field name and Nested keyword if missing
				if (!string.IsNullOrEmpty(fieldName) && !fieldName.EndsWith(" "))
				{
					fieldName += " ";
				}
				
				var formattedNested = FormatNestedStructure(nestedContent, indentLevel + 1);
				formattedFields.Add($"{fieldName}{formattedNested}");
			}
			else
			{
				formattedFields.Add(field.Trim());
			}
		}

		// Join fields with proper formatting
		var fieldsString = string.Join($",\n{baseIndent}", formattedFields);

		var reducedIndent = new string(' ', (indentLevel - 1) * 4);
		return $"Nested(\n{baseIndent}{fieldsString}\n{reducedIndent})";
	}
}
