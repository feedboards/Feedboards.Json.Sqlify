namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class FileNotFoundException : FeedboardsJsonSqlifyException
	{
		public FileNotFoundException(
			string filePath,
			string? reason = null,
			Exception? innerException = null)
			: base(
				ErrorCodes.FileNotFound,
				BuildMessage(filePath, reason),
				innerException,
				BuildMetadata(filePath, reason))
		{
		}

		private static string BuildMessage(string path, string? reason)
		{
			if (!string.IsNullOrWhiteSpace(reason))
			{
				return $"{ErrorCodes.GetErrorMessage(ErrorCodes.FileNotFound)}. Path: '{path}'. Reason: {reason}";
			}

			return $"{ErrorCodes.GetErrorMessage(ErrorCodes.FileNotFound)}. Path: '{path}'";
		}

		private static IDictionary<string, object> BuildMetadata(string path, string? reason)
		{
			var metadata = new Dictionary<string, object>
			{
				["FilePath"] = path
			};

			if (!string.IsNullOrWhiteSpace(reason))
			{
				metadata["Reason"] = reason;
			}

			return metadata;
		}
	}
}
