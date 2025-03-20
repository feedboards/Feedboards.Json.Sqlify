namespace Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text;

internal class ClickHouseSQLBuilder
{
	private bool HasNestedInNested(Dictionary<string, string> structure)
	{
		foreach (var kvp in structure)
		{
			if (kvp.Value.Contains("Nested("))
			{
				var nestedCount = kvp.Value.Split(new[] { "Nested(" }, StringSplitOptions.None).Length - 1;

				if (nestedCount > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GenerateClickHouseSchema(Dictionary<string, string> structure, string tableName)
	{
		var schemaLines = new List<string>();
		var processedFields = new HashSet<string>();
		var needsFlattenNested = HasNestedInNested(structure);

		var sqlBuilder = new StringBuilder();
		
		// Add SET flatten_nested=0 if we have nested in nested structures
		if (needsFlattenNested)
		{
			sqlBuilder.AppendLine("SET flatten_nested=0;");
			sqlBuilder.AppendLine();
		}

		foreach (var kvp in structure.OrderBy(kvp => kvp.Key))
		{
			string fieldName = kvp.Key;
			string fieldType = kvp.Value;

			// Skip if we've already processed this field or its parent
			if (processedFields.Any(f => fieldName.StartsWith(f + ".")))
			{
				continue;
			}

			if (!fieldName.Contains("."))
			{
				if (fieldType.StartsWith("Nested("))
				{
					string formattedType = FormatNestedStructure(fieldType);
					schemaLines.Add($"    `{fieldName}` {formattedType}");
					processedFields.Add(fieldName);
				}
				else
				{
					schemaLines.Add($"    `{fieldName}` {fieldType}");
					processedFields.Add(fieldName);
				}
			}
		}

		var schema = string.Join(",\n", schemaLines);

		sqlBuilder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName} (");
		sqlBuilder.AppendLine(schema);
		sqlBuilder.AppendLine(") ENGINE = MergeTree()");
		sqlBuilder.AppendLine("ORDER BY tuple();");

		return sqlBuilder.ToString();
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

		var formattedFields = new List<string>();
		var baseIndent = new string(' ', indentLevel * 4);

		foreach (var field in fields)
		{
			if (field.Contains("Nested("))
			{
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

		var fieldsString = string.Join($",\n{baseIndent}", formattedFields);
		var reducedIndent = new string(' ', (indentLevel - 1) * 4);
		
		return $"Nested(\n{baseIndent}{fieldsString}\n{reducedIndent})";
	}
}
