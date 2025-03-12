namespace Feedboards.Json.Sqlify.SQL.ClickHouse.Interfaces;

internal interface IClickHouseSQLBuilder
{
	string GenerateClickHouseSchema(Dictionary<string, string> structure, string table_name);
}
