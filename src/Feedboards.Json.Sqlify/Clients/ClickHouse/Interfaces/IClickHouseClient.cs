using Feedboards.Json.Sqlify.DTOs.ClickHouse;

namespace Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;

public interface IClickHouseClient
{
	public bool GenerateSQL(string tableName, int? maxDepth = 10);
	public bool GenerateSQL(string folderPath, FolderType folderType, string? tableName = null, int? maxDepth = 10);

	public bool GenerateSQL(string jsonFolder, string outputFolder, string tableName, int? maxDepth = 10);

	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null);

	public void GenerateSQLAndCreateTable(
		string? pathToFolderWithJson = null,
		string? PathToOutputFolder = null,
		ClickHouseDatabaseDetails? databaseDetails = null); //---
}
