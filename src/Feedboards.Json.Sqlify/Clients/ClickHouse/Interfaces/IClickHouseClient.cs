using Feedboards.Json.Sqlify.DTOs.ClickHouse;

namespace Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;

public interface IClickHouseClient
{
	public bool GenerateSQLAndWrite(string tableName);
	public bool GenerateSQLAndWrite(string folderPath, FolderType folderType, string? tableName = null);
	public bool GenerateSQLAndWrite(string jsonFolder, string outputFolder, string tableName);

	public string GenerateSQL(string tableName);
	public string GenerateSQL(string folderPath, string? tableName = null);

	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null);

	public void GenerateSQLAndCreateTable(
		string? pathToFolderWithJson = null,
		string? PathToOutputFolder = null,
		ClickHouseDatabaseDetails? databaseDetails = null); //---
}
