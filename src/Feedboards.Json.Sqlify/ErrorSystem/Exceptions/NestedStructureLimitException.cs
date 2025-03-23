namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class NestedStructureLimitException : FeedboardsJsonSqlifyException
	{
		public NestedStructureLimitException(
			int actualDepth,
			int maxAllowedDepth,
			string? tableName = null,
			string? nestedField = null,
			Exception? innerException = null)
			: base(
				errorCode: tableName != null ? ErrorCodes.NestedStructureLimit : ErrorCodes.JsonNestedStructureLimit,
				message: BuildMessage(actualDepth, maxAllowedDepth, tableName, nestedField),
				innerException,
				metadata: BuildMetadata(actualDepth, maxAllowedDepth, tableName, nestedField))
		{
		}

		private static string BuildMessage(int actual, int max, string? tableName = null, string? nestedField = null)
		{
			var baseMessage = ErrorCodes.GetErrorMessage(
				tableName != null ? ErrorCodes.NestedStructureLimit : ErrorCodes.JsonNestedStructureLimit);

			var details = new List<string>();

			details.Add($"Actual depth: {actual}");
			details.Add($"Maximum allowed: {max}");

			if (tableName != null)
			{
				details.Add($"Table: {tableName}");
			}

			if (nestedField != null)
			{
				details.Add($"Field: {nestedField}");
			}

			return $"{baseMessage}. {string.Join(", ", details)}";
		}

		private static IDictionary<string, object> BuildMetadata(int actual, int max, string? tableName = null, string? nestedField = null)
		{
			var metadata = new Dictionary<string, object>
			{
				["ActualDepth"] = actual,
				["MaxAllowedDepth"] = max
			};

			if (tableName != null)
			{
				metadata["TableName"] = tableName;
			}

			if (nestedField != null)
			{
				metadata["NestedField"] = nestedField;
			}

			return metadata;
		}
	}
}
