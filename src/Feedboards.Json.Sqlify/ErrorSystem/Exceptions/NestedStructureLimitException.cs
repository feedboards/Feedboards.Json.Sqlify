namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class NestedStructureLimitException : FeedboardsJsonSqlifyException
	{
		public NestedStructureLimitException(
			int? actualDepth = null,
			int? maxAllowedDepth = null,
			Exception? innerException = null)
			: base(
				ErrorCodes.NestedStructureLimit,
				BuildMessage(actualDepth, maxAllowedDepth),
				innerException,
				BuildMetadata(actualDepth, maxAllowedDepth))
		{
		}

		private static string BuildMessage(int? actual, int? max)
		{
			var baseMessage = ErrorCodes.GetErrorMessage(ErrorCodes.NestedStructureLimit);

			if (actual != null && max != null)
			{
				return $"{ErrorCodes.GetErrorMessage(ErrorCodes.NestedStructureLimit)}. Actual depth: {actual}, allowed maximum: {max}";
			}

			if (actual != null)
			{
				return $"{ErrorCodes.GetErrorMessage(ErrorCodes.NestedStructureLimit)}. Actual depth: {actual}";
			}

			if (max != null)
			{
				return $"{ErrorCodes.GetErrorMessage(ErrorCodes.NestedStructureLimit)}. Maximum allowed: {max}";
			}

			return baseMessage;
		}

		private static IDictionary<string, object>? BuildMetadata(int? actual, int? max)
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

			return metadata.Count > 0 ? metadata : null;
		}
	}
}
