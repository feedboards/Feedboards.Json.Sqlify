namespace Feedboards.Json.Sqlify.DTOs.ClickHouse;

public class ClickHouseOption
{
	public string? PathToOutputFolder { get; set; } = null;
	public string? PathToFolderWithJson {  get; set; } = null;
	public ClickHouseDatabaseDetails? DatabaseDetails { get; set; } = null;
}
