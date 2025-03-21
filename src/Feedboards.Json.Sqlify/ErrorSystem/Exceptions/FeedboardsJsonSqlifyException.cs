namespace Feedboards.Json.Sqlify.ErrorSystem.Exceptions
{
	public class FeedboardsJsonSqlifyException : Exception
	{
		private string errorCode;

		public string ErrorCode
		{
			get => errorCode;
			init => errorCode = value;
		}

		public IReadOnlyDictionary<string, object> Metadata { get; set; }

		public FeedboardsJsonSqlifyException(
			string errorCode,
			string message,
			Exception? innerException = null,
			IDictionary<string, object>? metadata = null)
			: base(message, innerException)
		{
			this.errorCode = errorCode;
			Metadata = metadata is null
				? new Dictionary<string, object>()
				: new Dictionary<string, object>(metadata);
		}

		public override string ToString()
		{
			var metaString = string.Join(", ", Metadata.Select(kv => $"{kv.Key}={kv.Value}"));
			return $"{base.ToString()} [Code={ErrorCode}, Metadata={{ {metaString} }}]";
		}
	}
}
