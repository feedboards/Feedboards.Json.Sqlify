namespace Feedboards.Json.Sqlify.DTOs.ClickHouse;

public class ClickHouseDatabaseDetails
{
	public required string Host { get; set; }
	public required short Port { get; set; }
	public required string User { get; set; }
	public required string Password { get; set; }
	public required string Database { get; set; }
}
