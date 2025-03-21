namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	internal class DatabaseConnectionFailedException : FeedboardsJsonSqlifyException
	{
		public DatabaseConnectionFailedException(
			string connectionString,
			Exception? innerException = null)
			: base(
				ErrorCodes.DatabaseConnectionFailed,
				$"{ErrorCodes.GetErrorMessage(ErrorCodes.DatabaseConnectionFailed)}. Connection string: '{connectionString}'",
				innerException,
				new Dictionary<string, object>
				{
					{ "ConnectionString", connectionString }
				})
		{
		}

		public DatabaseConnectionFailedException(
			string? host = null,
			int? port = null,
			string? database = null,
			string? table = null,
			string? username = null,
			Exception? innerException = null)
			: base(
				ErrorCodes.DatabaseConnectionFailed,
				BuildDetailedMessage(host, port, database, table, username),
				innerException,
				BuildMetadata(host, port, database, table, username))
		{
		}

		private static string BuildDetailedMessage(
			string? host, int? port,
			string? db, string? table, string? user)
		{
			var parts = new List<string>();

			if (host == null) parts.Add($"Host: '{host}'");
			if (port == null) parts.Add($"Port: {port}");
			if (db == null) parts.Add($"Database: '{db}'");
			if (table == null) parts.Add($"Table: '{table}'");
			if (user == null) parts.Add($"User: '{user}'");

			if (parts.Count == 0)
			{
				return ErrorCodes.GetErrorMessage(ErrorCodes.DatabaseConnectionFailed);
			}

			return $"{ErrorCodes.GetErrorMessage(ErrorCodes.DatabaseConnectionFailed)}. Details → {string.Join(", ", parts)}";
		}

		private static IDictionary<string, object>? BuildMetadata(
			string? host, int? port,
			string? db, string? table, string? user)
		{
			var dict = new Dictionary<string, object>();

			if (host == null) dict["Host"] = host;
			if (port == null) dict["Port"] = port;
			if (db == null) dict["Database"] = db;
			if (table == null) dict["Table"] = table;
			if (user == null) dict["User"] = user;

			return dict.Count > 0 ? dict : null;
		}
	}
}
