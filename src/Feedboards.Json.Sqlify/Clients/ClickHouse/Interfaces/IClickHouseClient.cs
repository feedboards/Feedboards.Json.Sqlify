using Feedboards.Json.Sqlify.DTOs.ClickHouse;

namespace Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;

public interface IClickHouseClient : ISQL
{
	/// <summary>
	/// Creates a table in ClickHouse database using the generated schema.
	/// </summary>
	/// <param name="databaseDetails">Connection details for the ClickHouse database</param>
	/// <returns>True if the table was created successfully</returns>
	/// <exception cref="NotImplementedException">This method is not yet implemented</exception>
	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null);

	/// <summary>
	/// Generates SQL schema from JSON data and creates a table in ClickHouse database.
	/// </summary>
	/// <param name="pathToFolderWithJson">Path to the JSON file or folder</param>
	/// <param name="PathToOutputFolder">Path to the output SQL file or folder</param>
	/// <param name="databaseDetails">Connection details for the ClickHouse database</param>
	/// <exception cref="NotImplementedException">This method is not yet implemented</exception>
	public void GenerateSQLAndCreateTable(
		string? pathToFolderWithJson = null,
		string? PathToOutputFolder = null,
		ClickHouseDatabaseDetails? databaseDetails = null);
}
