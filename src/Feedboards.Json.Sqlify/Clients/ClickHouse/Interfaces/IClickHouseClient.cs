using Feedboards.Json.Sqlify.DTOs.ClickHouse;

namespace Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;

public interface IClickHouseClient
{
	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses configuration options for both input and output paths.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when PathToFolderWithJson or PathToOutputFolder is not provided in options</exception>
	public bool GenerateSQLAndWrite(string tableName);

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses the provided folder path and configuration for the other folder.
	/// </summary>
	/// <param name="folderPath">Path to the folder containing JSON files or output folder</param>
	/// <param name="folderType">Type of the provided folder (JsonFolder or OutputFolder)</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when the required configuration option is not provided</exception>
	public bool GenerateSQLAndWrite(string folderPath, FolderType folderType, string? tableName = null);

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses provided paths for both input and output.
	/// </summary>
	/// <param name="jsonFolder">Path to the JSON file or folder</param>
	/// <param name="outputFolder">Path to the output SQL file or folder</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when paths are invalid or when tableName is null</exception>
	/// <exception cref="InvalidTableNameException">Thrown when tableName is null or empty</exception>
	/// <exception cref="InvalidConfigurationException">Thrown when maxDepth is null</exception>
	/// <exception cref="CustomFileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	/// <exception cref="FeedboardsJsonSqlifyException">Thrown when an unexpected error occurs</exception>
	public bool GenerateSQLAndWrite(string jsonFolder, string outputFolder, string tableName);

	/// <summary>
	/// Generates SQL schema from JSON data and returns it as a string.
	/// Uses configuration options for input path.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when PathToFolderWithJson is not provided in options</exception>
	public string GenerateSQL(string tableName);

	/// <summary>
	/// Generates SQL schema from JSON data and returns it as a string.
	/// Uses the provided JSON file path.
	/// </summary>
	/// <param name="jsonFolder">Path to the JSON file</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="InvalidTableNameException">Thrown when tableName is null or empty</exception>
	/// <exception cref="InvalidConfigurationException">Thrown when maxDepth is null</exception>
	/// <exception cref="CustomFileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	public string GenerateSQL(string folderPath, string? tableName = null);

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
