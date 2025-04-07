namespace Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;

internal class ClickHouseSQLBuilder
{
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

	private void ValidateSQLNesting(Dictionary<string, string> structure, string tableName, int maxDepth)
	{
		if (maxDepth <= 0) return; // Skip validation for unlimited depth

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

		// Format and add top-level fields
		for (int i = 0; i < topLevelFields.Count; i++)
		{
			string fieldName = topLevelFields[i].Key;
			string fieldType = topLevelFields[i].Value;

			// Format the field type
			string formattedField = $"    `{fieldName}` {CleanAndFormatType(fieldType)}";
			
			// Add comma if not the last field
			if (i < topLevelFields.Count - 1)
				sqlBuilder.AppendLine(formattedField + ",");
			else
				sqlBuilder.AppendLine(formattedField);
		}

		// Close statement
		sqlBuilder.AppendLine(")");
		sqlBuilder.AppendLine("ENGINE = MergeTree()");
		sqlBuilder.AppendLine("ORDER BY tuple();");

		return sqlBuilder.ToString();
	}

	// Process and format types with proper indentation
	private string CleanAndFormatType(string fieldType)
	{
		// Handle Nested structures
		if (fieldType.Contains("Nested("))
		{
			// Fix the field names and structure to match the desired format
			return FormatNestedTypeRecursively(fieldType, 1); // Start with indentation level 1
		}
		
		return fieldType;
	}
	
	// Format nested type string with proper indentation recursively
	private string FormatNestedTypeRecursively(string type, int indentationLevel)
	{
		// Check if it's a Nullable type
		bool isNullable = type.StartsWith("Nullable(");
		string actualType = type;
		string nullablePrefix = "";
		
		if (isNullable)
		{
			// Extract the actual nested type from Nullable(...)
			nullablePrefix = "Nullable(";
			actualType = type.Substring(9, type.Length - 10); 
		}
		
		// Not a nested structure, return as is with wrapper if needed
		if (!actualType.StartsWith("Nested("))
			return type;
			
		// Parse the nested structure
		string content = ExtractNestedContent(actualType);
		List<(string name, string type)> fields = ParseNestedFields(content);
		
		// Build the formatted string with proper indentation
		var result = new StringBuilder();
		
		// Add the Nullable wrapper if needed
		if (isNullable)
			result.Append(nullablePrefix);
			
		// Start the Nested structure
		result.AppendLine("Nested(");
		
		// Format each field
		string indent = new string(' ', 4 * (indentationLevel + 1));
		for (int i = 0; i < fields.Count; i++)
		{
			var (name, fieldType) = fields[i];
			result.Append(indent).Append('`').Append(name).Append("` ");
			
			// Check if this field's type is also a nested structure
			if (fieldType.Contains("Nested("))
			{
				// Handle nested field recursively
				string nestedFormatted = FormatNestedTypeRecursively(fieldType, indentationLevel + 1);
				result.Append(nestedFormatted);
			}
			else
			{
				// Simple type
				result.Append(fieldType);
			}
			
			// Add comma if not the last field
			if (i < fields.Count - 1)
				result.AppendLine(",");
			else
				result.AppendLine();
		}
		
		// Close the Nested structure
		result.Append(new string(' ', 4 * indentationLevel)).Append(')');
		
		// Close Nullable wrapper if needed
		if (isNullable)
			result.Append(')');
			
		return result.ToString();
	}
	
	// Extract the content inside Nested( )
	private string ExtractNestedContent(string nestedType)
	{
		int start = nestedType.IndexOf("Nested(") + 7;
		int end = nestedType.LastIndexOf(')');
		return nestedType.Substring(start, end - start);
	}
	
	// Parse fields from nested content
	private List<(string name, string type)> ParseNestedFields(string content)
	{
		List<(string name, string type)> fields = new List<(string name, string type)>();
		if (string.IsNullOrWhiteSpace(content))
			return fields;
			
		int pos = 0;
		while (pos < content.Length)
		{
			// Find the field name (enclosed in backticks)
			int nameStart = content.IndexOf('`', pos);
			if (nameStart == -1) break;
			
			int nameEnd = content.IndexOf('`', nameStart + 1);
			if (nameEnd == -1) break;
			
			string name = content.Substring(nameStart + 1, nameEnd - nameStart - 1);
			
			// Skip the closing backtick and whitespace
			int typeStart = nameEnd + 1;
			while (typeStart < content.Length && char.IsWhiteSpace(content[typeStart]))
				typeStart++;
				
			// Find where this field's type ends
			int typeEnd = FindFieldTypeEnd(content, typeStart);
			string type = content.Substring(typeStart, typeEnd - typeStart).Trim();
			
			// Add to our list
			fields.Add((name, type));
			
			// Move past this field
			pos = typeEnd;
		}
		
		return fields;
	}
	
	// Find the end of a field type, accounting for nested structures
	private int FindFieldTypeEnd(string content, int startPos)
	{
		int pos = startPos;
		int parenCount = 0;
		bool insideType = true;
		
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
				// End of this field if we hit a comma at top level
				return pos;
			}
			else if (c == '`' && parenCount == 0)
			{
				// If we encounter a backtick and we're not inside parentheses,
				// that's the start of the next field name
				return pos;
			}
			
			pos++;
		}
		
		// If we get here, we reached the end of the content
		return content.Length;
	}
}
