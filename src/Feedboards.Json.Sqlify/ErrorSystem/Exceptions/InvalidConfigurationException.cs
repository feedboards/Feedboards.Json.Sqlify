namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class InvalidConfigurationException : FeedboardsJsonSqlifyException
	{
		public InvalidConfigurationException(string configKey, string message)
			: base(
				  ErrorCodes.InvalidConfiguration,
				  $"{ErrorCodes.GetErrorMessage(ErrorCodes.InvalidConfiguration)}. {message}",
				  null,
				  metadata: new Dictionary<string, object> { ["ConfigKey"] = configKey })
		{
		}

		public InvalidConfigurationException(string configKey)
			: base(
				  ErrorCodes.InvalidConfiguration,
				  $"{ErrorCodes.GetErrorMessage(ErrorCodes.InvalidConfiguration)}. The key '{configKey}' is invalid or missing.",
				  null,
				  metadata: new Dictionary<string, object> { ["ConfigKey"] = configKey })
		{
		}
	}
}
