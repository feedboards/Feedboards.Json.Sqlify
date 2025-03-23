namespace Feedboards.Json.Sqlify.DTOs.ClickHouse;

public class ClickHouseOption
{
	public string? PathToOutputFolder { get; set; } = null;
	public string? PathToFolderWithJson {  get; set; } = null;
	public ClickHouseDatabaseDetails? DatabaseDetails { get; set; } = null;
	public int MaxDepth { get; set; } = 10; // Default to 10 levels of nesting
}
