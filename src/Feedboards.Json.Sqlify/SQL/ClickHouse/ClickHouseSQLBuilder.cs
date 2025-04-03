namespace Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;

internal class ClickHouseSQLBuilder
{
	private bool HasNestedInNested(Dictionary<string, string> structure)
	{
		foreach (var kvp in structure)
		{
			if (kvp.Value.Contains("Array(") || kvp.Value.Contains("Tuple("))
			{
				var nestedCount = kvp.Value.Split(new[] { "Array(", "Tuple(" }, StringSplitOptions.None).Length - 1;

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
			if (kvp.Value.Contains("Array(") || kvp.Value.Contains("Tuple("))
			{
				var nestedCount = kvp.Value.Split(new[] { "Array(", "Tuple(" }, StringSplitOptions.None).Length - 1;
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

		// Format CREATE TABLE statement
		sqlBuilder.AppendLine($"CREATE TABLE IF NOT EXISTS {tableName}");
		sqlBuilder.AppendLine("(");

		// Format fields with proper indentation
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
				// Format field type for better readability
				var formattedType = FormatFieldType(fieldType);
				schemaLines.Add($"    `{fieldName}` {formattedType}");
				processedFields.Add(fieldName);
			}
		}

		// Join fields with proper line breaks and indentation
		sqlBuilder.AppendLine(string.Join(",\n", schemaLines));
		sqlBuilder.AppendLine(")");
		sqlBuilder.AppendLine("ENGINE = MergeTree()");
		sqlBuilder.AppendLine("ORDER BY tuple();");

		return sqlBuilder.ToString();
	}

	private string FormatFieldType(string fieldType)
	{
		if (fieldType.StartsWith("Nested("))
		{
			// Format Nested type
			var innerContent = fieldType.Substring(7, fieldType.Length - 8); // Remove "Nested(" and ")"
			var fields = innerContent.Split(',').Select(f => f.Trim());
			var formattedFields = new List<string>();
			foreach (var field in fields)
			{
				if (field.StartsWith("Nested("))
				{
					// Handle nested Nested types
					var nestedContent = field.Substring(7, field.Length - 8);
					var nestedFields = nestedContent.Split(',').Select(f => f.Trim());
					formattedFields.Add($"Nested({string.Join(", ", nestedFields)})");
				}
				else if (field.StartsWith("Array("))
				{
					formattedFields.Add(field);
				}
				else if (field.StartsWith("Tuple("))
				{
					var tupleContent = field.Substring(6, field.Length - 7);
					var tupleFields = tupleContent.Split(',').Select(f => f.Trim());
					formattedFields.Add($"Tuple({string.Join(", ", tupleFields)})");
				}
				else
				{
					formattedFields.Add(field);
				}
			}
			return $"Nested(\n        {string.Join(",\n        ", formattedFields)}\n    )";
		}
		else if (fieldType.StartsWith("Tuple("))
		{
			// Format Tuple type
			var innerContent = fieldType.Substring(6, fieldType.Length - 7); // Remove "Tuple(" and ")"
			var fields = innerContent.Split(',').Select(f => f.Trim());
			return $"Tuple({string.Join(", ", fields)})";
		}
		else if (fieldType.StartsWith("Array("))
		{
			// Format Array type
			var innerContent = fieldType.Substring(5, fieldType.Length - 6); // Remove "Array(" and ")"
			if (innerContent.StartsWith("Tuple(") || innerContent.StartsWith("Nested("))
			{
				// For nested types inside Array, format them recursively
				return $"Array({FormatFieldType(innerContent)})";
			}
			return $"Array({innerContent})";
		}
		return fieldType;
	}
}
