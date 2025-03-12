namespace Feedboards.Json.Sqlify.DTOs.ClickHouse;

public class ClickHouseOption
{
	public bool CreateTableInDatabase { get; set; } = false;
	public bool CreateSQLFile { get; set; } = true;
	public ClickHouseDatabaseDetails? DatabaseDetails { get; set; } = null;
}
