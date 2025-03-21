namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class InvalidJsonStructureException : FeedboardsJsonSqlifyException
	{
		public InvalidJsonStructureException(string? jsonPath = null, Exception? innerException = null)
			: base(
				ErrorCodes.InvalidJsonStructure,
				jsonPath == null
					? ErrorCodes.GetErrorMessage(ErrorCodes.InvalidJsonStructure)
					: $"{ErrorCodes.GetErrorMessage(ErrorCodes.InvalidJsonStructure)} at path '{jsonPath}'",
				innerException,
				metadata: jsonPath != null
					? new Dictionary<string, object> { ["JsonPath"] = jsonPath }
					: null)
		{
		}
	}
}
