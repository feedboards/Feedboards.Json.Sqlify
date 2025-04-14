namespace Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;

public interface ISQL : IDisposable
{
	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses configuration options for both input and output paths.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
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
	/// <exception cref="CustomFileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	/// <exception cref="FeedboardsJsonSqlifyException">Thrown when an unexpected error occurs</exception>
	public bool GenerateSQLAndWrite(string jsonFolder, string outputFolder, string tableName);

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses provided paths for both input and output.
	/// </summary>
	/// <param name="stream">Stream of the JSON</param>
	/// <param name="outputFolder">Path to the output SQL file</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when paths are invalid or when tableName is null</exception>
	/// <exception cref="InvalidTableNameException">Thrown when tableName is null or empty</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	/// <exception cref="FeedboardsJsonSqlifyException">Thrown when an unexpected error occurs</exception>
	public bool GenerateSQLAndWrite(Stream stream, string outputFolder, string tableName);

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
	/// <exception cref="CustomFileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	public string GenerateSQL(string folderPath, string? tableName = null);

	/// Generates SQL schema from FileStream, MemoryStream or Stream and returns it as a string.
	/// Uses the provided FileStream, MemoryStream or Stream.
	/// </summary>
	/// <param name="stream">Stream of the JSON file</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="InvalidTableNameException">Thrown when tableName is null or empty</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	public string GenerateSQL(Stream stream, string tableName);
}
