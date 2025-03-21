namespace Feedboards.Json.Sqlify.ErrorSystem
{
	public class ErrorCodes
	{
		public static string GetErrorMessage(string errorCode) => errorCode switch
		{
			InvalidJsonStructure => "Invalid JSON structure detected",
			InvalidConfiguration => "Invalid configuration provided",
			FileNotFound => "File not found at specified path", //
			InvalidTableName => "Invalid table name provided",
			NestedStructureLimit => "Nested structure exceeds maximum depth",
			DatabaseConnectionFailed => "Failed to connect to database",
			UnknownError => "An unknown error occurred",
			_ => "An unknown error occurred"
		};

		public const string FileNotFound = "FILE_001";
		public const string InvalidConfiguration = "CFG_001";
		public const string InvalidTableName = "TBL_001";
		public const string InvalidJsonStructure = "JSN_001";
		public const string DatabaseConnectionFailed = "DB_001";
		public const string UnknownError = "UNK_001";
		public const string NestedStructureLimit = "SQL_001";
	}
}
