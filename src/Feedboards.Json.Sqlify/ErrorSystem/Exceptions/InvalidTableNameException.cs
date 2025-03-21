namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class InvalidTableNameException : FeedboardsJsonSqlifyException
	{
		public InvalidTableNameException(
			string tableName,
			string? databaseName = null,
			Exception? innerException = null)
			: base(
				ErrorCodes.InvalidTableName,
				BuildMessage(tableName, databaseName),
				innerException,
				BuildMetadata(tableName, databaseName))
		{
		}

		private static string BuildMessage(string tableName, string? dbName)
		{
			if (string.IsNullOrWhiteSpace(dbName))
			{
				return $"{ErrorCodes.GetErrorMessage(ErrorCodes.InvalidTableName)}. Table name: '{tableName}'";
			}

			return $"{ErrorCodes.GetErrorMessage(ErrorCodes.InvalidTableName)}. Table: '{tableName}' in database: '{dbName}'";
		}

		private static IDictionary<string, object> BuildMetadata(string tableName, string? dbName)
		{
			var metadata = new Dictionary<string, object>
			{
				["TableName"] = tableName
			};

			if (!string.IsNullOrWhiteSpace(dbName))
			{
				metadata["DatabaseName"] = dbName;
			}

			return metadata;
		}
	}
}
