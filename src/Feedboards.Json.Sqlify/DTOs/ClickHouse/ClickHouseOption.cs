namespace Feedboards.Json.Sqlify.DTOs.ClickHouse;

public class ClickHouseOption
{
	public string? PathToOutputFolder { get; set; } = string.Empty;
	public string? PathToFolderWithJson {  get; set; } = string.Empty;
	public ClickHouseDatabaseDetails? DatabaseDetails { get; set; } = null;
}
