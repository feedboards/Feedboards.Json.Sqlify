namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class NestedStructureLimitException : FeedboardsJsonSqlifyException
	{
		public NestedStructureLimitException(
			int? actualDepth = null,
			int? maxAllowedDepth = null,
			string? tableName = null,
			string? nestedField = null,
			Exception? innerException = null)
			: base(
				errorCode: tableName != null ? ErrorCodes.NestedStructureLimit : ErrorCodes.InvalidJsonStructure,
				message: BuildMessage(actualDepth, maxAllowedDepth, tableName, nestedField),
				innerException,
				metadata: BuildMetadata(actualDepth, maxAllowedDepth, tableName, nestedField))
		{
		}

		private static string BuildMessage(int? actual, int? max, string? tableName = null, string? nestedField = null)
		{
			var baseMessage = ErrorCodes.GetErrorMessage(tableName != null ? ErrorCodes.NestedStructureLimit : ErrorCodes.InvalidJsonStructure);

			var details = new List<string>();

			if (actual != null)
			{
				details.Add($"Actual depth: {actual}");
			}

			if (max != null)
			{
				details.Add($"Maximum allowed: {max}");
			}

			if (tableName != null)
			{
				details.Add($"Table: {tableName}");
			}

			if (nestedField != null)
			{
				details.Add($"Field: {nestedField}");
			}

			return details.Count > 0
				? $"{baseMessage}. {string.Join(", ", details)}"
				: baseMessage;
		}

		private static IDictionary<string, object>? BuildMetadata(int? actual, int? max, string? tableName = null, string? nestedField = null)
		{
			var metadata = new Dictionary<string, object>();

			if (actual != null)
			{
				metadata["ActualDepth"] = actual;
			}

			if (max != null)
			{
				metadata["MaxAllowedDepth"] = max;
			}

			if (tableName != null)
			{
				metadata["TableName"] = tableName;
			}

			if (nestedField != null)
			{
				metadata["NestedField"] = nestedField;
			}

			return metadata.Count > 0 ? metadata : null;
		}
	}
}
