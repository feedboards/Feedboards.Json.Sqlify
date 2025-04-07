namespace Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;

internal class ClickHouseSQLBuilder
{
	public string GenerateClickHouseSchema(Dictionary<string, string> structure, string tableName, int maxDepth = 10)
	{
		ValidateSQLNesting(structure, tableName, maxDepth);

		var needsFlattenNested = HasNestedInNested(structure);

		var sqlBuilder = new StringBuilder();
		
		// Add SET flatten_nested=0 if we have nested in nested structures
		if (needsFlattenNested)
		{
			sqlBuilder.AppendLine("SET flatten_nested=0;");
			sqlBuilder.AppendLine();
		}

		// Format CREATE TABLE statement
		sqlBuilder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName}");
		sqlBuilder.AppendLine("(");

		// Get top-level fields
		var topLevelFields = structure
			.Where(kvp => !kvp.Key.Contains("."))
			.OrderBy(kvp => kvp.Key)
			.ToList();

		for (int i = 0; i < topLevelFields.Count; i++)
		{
			var fieldName = topLevelFields[i].Key;
			var fieldType = topLevelFields[i].Value;

			var formattedField = $"    `{fieldName}` {CleanAndFormatType(fieldType)}";
			
			if (i < topLevelFields.Count - 1)
			{
				sqlBuilder.AppendLine(formattedField + ",");
			}
			else
			{
				sqlBuilder.AppendLine(formattedField);
			}
		}

		sqlBuilder.AppendLine(")");
		sqlBuilder.AppendLine("ENGINE = MergeTree()");
		sqlBuilder.AppendLine("ORDER BY tuple();");

		return sqlBuilder.ToString();
	}

	private bool HasNestedInNested(Dictionary<string, string> structure)
	{
		foreach (var kvp in structure)
		{
			if (
				kvp.Value.Contains("Array(") ||
				kvp.Value.Contains("Tuple(") ||
				kvp.Value.Contains("Nested("))
			{
				var nestedCount = kvp.Value.Split(new[] { "Array(", "Tuple(", "Nested(" }, StringSplitOptions.None).Length - 1;

				if (nestedCount > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ValidateSQLNesting(
		Dictionary<string, string> structure,
		string tableName,
		int maxDepth)
	{
		if (maxDepth <= 0)
		{
			return;
		}

		foreach (var kvp in structure)
		{
			if (
				kvp.Value.Contains("Array(") ||
				kvp.Value.Contains("Tuple(") ||
				kvp.Value.Contains("Nested("))
			{
				var nestedCount = kvp.Value.Split(new[] { "Array(", "Tuple(", "Nested(" }, StringSplitOptions.None).Length - 1;
				if (nestedCount > maxDepth)
				{
					throw new NestedStructureLimitException(
						actualDepth: nestedCount,
						maxAllowedDepth: maxDepth,
						tableName: tableName,
						nestedField: kvp.Key);
				}
			}
		}
	}

	private string CleanAndFormatType(string fieldType)
	{
		if (fieldType.Contains("Nested("))
		{
			return FormatNestedTypeRecursively(fieldType, 1);
		}
		
		return fieldType;
	}
	
	private string FormatNestedTypeRecursively(string type, int indentationLevel)
	{
		var isNullable = type.StartsWith("Nullable(");
		var actualType = type;
		var nullablePrefix = "";
		
		if (isNullable)
		{
			nullablePrefix = "Nullable(";
			actualType = type.Substring(9, type.Length - 10); 
		}
		
		if (!actualType.StartsWith("Nested("))
		{
			return type;
		}

		var content = ExtractNestedContent(actualType);
		var fields = ParseNestedFields(content);
		
		var result = new StringBuilder();
		
		if (isNullable)
		{
			result.Append(nullablePrefix);
		}	
		
		result.AppendLine("Nested(");
		
		string indent = new string(' ', 4 * (indentationLevel + 1));
		for (int i = 0; i < fields.Count; i++)
		{
			var (name, fieldType) = fields[i];
			result.Append(indent).Append('`').Append(name).Append("` ");
			
			if (fieldType.Contains("Nested("))
			{
				string nestedFormatted = FormatNestedTypeRecursively(fieldType, indentationLevel + 1);
				result.Append(nestedFormatted);
			}
			else
			{
				result.Append(fieldType);
			}
			
			if (i < fields.Count - 1)
			{
				result.AppendLine(",");
			}
			else
			{
				result.AppendLine();
			}
		}
		
		result.Append(new string(' ', 4 * indentationLevel)).Append(')');
		
		if (isNullable)
		{
			result.Append(')');
		}
			
		return result.ToString();
	}
	
	private string ExtractNestedContent(string nestedType)
	{
		var start = nestedType.IndexOf("Nested(") + 7;
		var end = nestedType.LastIndexOf(')');
		return nestedType.Substring(start, end - start);
	}
	
	private List<(string name, string type)> ParseNestedFields(string content)
	{
		var fields = new List<(string name, string type)>();
		if (string.IsNullOrWhiteSpace(content))
		{
			return fields;
		}
			
		var pos = 0;
		while (pos < content.Length)
		{
			var nameStart = content.IndexOf('`', pos);
			if (nameStart == -1)
			{
				break;
			}
			
			var nameEnd = content.IndexOf('`', nameStart + 1);
			if (nameEnd == -1)
			{
				break;
			}
			
			var name = content.Substring(nameStart + 1, nameEnd - nameStart - 1);
			
			var typeStart = nameEnd + 1;
			while (typeStart < content.Length && char.IsWhiteSpace(content[typeStart]))
			{
				typeStart++;
			}
				
			var typeEnd = FindFieldTypeEnd(content, typeStart);
			var type = content.Substring(typeStart, typeEnd - typeStart).Trim();
			
			fields.Add((name, type));
			
			pos = typeEnd;
		}
		
		return fields;
	}
	
	private int FindFieldTypeEnd(string content, int startPos)
	{
		var pos = startPos;
		var parenCount = 0;
		var insideType = true;
		
		while (pos < content.Length && insideType)
		{
			char c = content[pos];
			
			if (c == '(')
			{
				parenCount++;
			}
			else if (c == ')')
			{
				parenCount--;
			}
			else if (c == ',' && parenCount == 0)
			{
				return pos;
			}
			else if (c == '`' && parenCount == 0)
			{
				return pos;
			}
			
			pos++;
		}
		
		return content.Length;
	}
}
