using Feedboards.Json.Sqlify.Clients.ClickHouse;
using Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;
using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using Feedboards.Json.Sqlify.ErrorSystem;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;
using Feedboards.Json.Sqlify.JSON.ClickHouse;
using Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text.Json;
using System.Text.RegularExpressions;
using CustomFileNotFoundException = Feedboards.Json.Sqlify.ErrorSystem.Exceptions.FileNotFoundException;

namespace Feedboards.Json.Sqlify.Clients.ClickHousel;

public class ClickHouseClient : IClickHouseClient
{
	private readonly ClickHouseOption? option;
	private static readonly Regex ValidTableNameRegex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

	public ClickHouseClient(ClickHouseOption? option = null)
	{
		this.option = option;
		if (option != null && string.IsNullOrEmpty(option.PathToFolderWithJson))
		{
			throw new InvalidConfigurationException("PathToFolderWithJson", "Path to JSON folder is required");
		}
	}

	/// <summary>
	/// Generates SQL schema from JSON data and returns it as a string.
	/// Uses configuration options for input path.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when PathToFolderWithJson is not provided in options</exception>
	public string GenerateSQL(string tableName)
	{
		var jsonFolder = option?.PathToFolderWithJson
			?? throw new InvalidConfigurationException("PathToFolderWithJson", "Path to JSON folder is required");

		return GenerateSQL(jsonFolder, tableName);
	}

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
	public string GenerateSQL(string jsonFolder, string? tableName = null)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			throw new InvalidTableNameException(tableName ?? "null");
		}

		if (!ValidTableNameRegex.IsMatch(tableName))
		{
			throw new InvalidTableNameException(tableName);
		}

		try
		{
			var jsonStream = File.OpenRead(jsonFolder);

			return GenerateSQL(jsonStream, tableName);
		}
		catch (System.IO.FileNotFoundException ex)
		{
			throw new CustomFileNotFoundException(jsonFolder, "File does not exist", ex);
		}
		catch (DirectoryNotFoundException ex)
		{
			throw new CustomFileNotFoundException(jsonFolder, "Directory does not exist", ex);
		}
		catch (JsonException ex)
		{
			throw new InvalidJsonStructureException(jsonFolder, ex);
		}
	}

	/// <summary>
	/// Generates SQL schema from FileStream, MemoryStream or Stream and returns it as a string.
	/// Uses the provided FileStream, MemoryStream or Stream.
	/// </summary>
	/// <param name="stream">Stream of the JSON file</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="InvalidTableNameException">Thrown when tableName is null or empty</exception>
	/// <exception cref="InvalidJsonStructureException">Thrown when the JSON file contains invalid JSON</exception>
	public string GenerateSQL(Stream stream, string tableName)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			throw new InvalidTableNameException(tableName);
		}

		if (!ValidTableNameRegex.IsMatch(tableName))
		{
			throw new InvalidTableNameException(tableName);
		}

		try
		{
			using var document = JsonDocument.Parse(stream);

			var jsonData = document.RootElement;

			var jsonAnalyzer = new ClickHouseJsonAnalyzer();
			var sqlBuilder = new ClickHouseSQLBuilder();

			var structure = jsonAnalyzer.AnalyzeJsonStructure(jsonData, "");

			return sqlBuilder.GenerateSchema(structure, tableName);
		}
		catch (System.IO.FileNotFoundException ex)
		{
			throw new CustomFileNotFoundException("FileSteam", "File does not exist", ex);
		}
		catch (DirectoryNotFoundException ex)
		{
			throw new CustomFileNotFoundException("FileSteam", "Directory does not exist", ex);
		}
		catch (JsonException ex)
		{
			throw new InvalidJsonStructureException("FileSteam", ex);
		}
	}

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses configuration options for both input and output paths.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when PathToFolderWithJson or PathToOutputFolder is not provided in options</exception>
	public bool GenerateSQLAndWrite(string tableName)
	{
		var jsonFolder = option?.PathToFolderWithJson
			?? throw new InvalidConfigurationException("PathToFolderWithJson");
		var outputFolder = option?.PathToOutputFolder
			?? throw new InvalidConfigurationException("PathToOutputFolder");

		return GenerateSQLAndWrite(jsonFolder, outputFolder, tableName);
	}

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses the provided folder path and configuration for the other folder.
	/// </summary>
	/// <param name="folderPath">Path to the folder containing JSON files or output folder</param>
	/// <param name="folderType">Type of the provided folder (JsonFolder or OutputFolder)</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="InvalidConfigurationException">Thrown when the required configuration option is not provided</exception>
	public bool GenerateSQLAndWrite(string folderPath, FolderType folderType, string? tableName = null)
	{
		if (folderType == FolderType.JsonFolder)
		{
			var outputFolder = option?.PathToOutputFolder
				?? throw new InvalidConfigurationException("PathToOutputFolder");

			return GenerateSQLAndWrite(folderPath, outputFolder);
		}
		else // FolderType.OutputFolder
		{
			var jsonFolder = option?.PathToFolderWithJson
				?? throw new InvalidConfigurationException("PathToFolderWithJson");

			return GenerateSQLAndWrite(jsonFolder, folderPath, tableName);
		}
	}

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
	public bool GenerateSQLAndWrite(string jsonFolder, string outputFolder, string? tableName = null)
	{
		var jsonFolderType = Utils.CheckPath(jsonFolder);
		var outputFolderType = Utils.CheckPath(outputFolder);

		if (jsonFolderType != outputFolderType)
		{
			var metadata = new Dictionary<string, object>
			{
				["Message"] = "Both arguments must be of the same type. " +
							"Please provide either two folder paths or two file paths. " +
							"Mixing a folder path with a file path is not allowed",
				["JsonFolderType"] = jsonFolderType.ToString(),
				["OutputFolderType"] = outputFolderType.ToString()
			};

			throw new FeedboardsJsonSqlifyException(
				ErrorCodes.InvalidConfiguration,
				"Invalid path combination",
				null,
				metadata);
		}

		try
		{
			if (jsonFolderType == FileOrFolderChecker.Folder)
			{
				foreach (var file in Directory.GetFiles(jsonFolder))
				{
					var fileName = Path.GetFileName(file).Split('.')[0];
					var sqlFileName = Path.GetFileName(file).Split('.')[0] + ".sql";

					GenerateSQLAndWrite(
						file,
						Path.Combine(outputFolder, sqlFileName),
						fileName);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(tableName))
				{
					throw new InvalidTableNameException(tableName ?? "null");
				}

				var outputPath = Path.GetFullPath(outputFolder);
				File.WriteAllText(
					outputPath,
					GenerateSQL(
						jsonFolder,
						tableName));
			}

			return true;
		}
		catch (Exception exc) when (exc is not FeedboardsJsonSqlifyException)
		{
			throw new FeedboardsJsonSqlifyException(
				ErrorCodes.UnknownError,
				"An unexpected error occurred while processing the files",
				exc,
				new Dictionary<string, object>
				{
					["JsonFolder"] = jsonFolder,
					["OutputFolder"] = outputFolder,
					["TableName"] = tableName ?? "null",
				});
		}
	}

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
	public bool GenerateSQLAndWrite(Stream stream, string outputFolder, string tableName)
	{
		var outputFolderType = Utils.CheckPath(outputFolder);

		if (outputFolderType == FileOrFolderChecker.Folder)
		{
			var metadata = new Dictionary<string, object>
			{
				["Message"] = "outputFolder argument must be of the file type.",
				["FileStream"] = stream,
				["OutputFolderType"] = outputFolderType.ToString()
			};

			throw new FeedboardsJsonSqlifyException(
				ErrorCodes.InvalidConfiguration,
				"Invalid path combination",
				null,
				metadata);
		}

		try
		{
			if (string.IsNullOrEmpty(tableName))
			{
				throw new InvalidTableNameException(tableName ?? "null");
			}

			var outputPath = Path.GetFullPath(outputFolder);
			File.WriteAllText(
				outputPath,
				GenerateSQL(
					stream,
					tableName));

			return true;
		}
		catch (Exception exc) when (exc is not FeedboardsJsonSqlifyException)
		{
			throw new FeedboardsJsonSqlifyException(
				ErrorCodes.UnknownError,
				"An unexpected error occurred while processing the files",
				exc,
				new Dictionary<string, object>
				{
					["FileStream"] = stream,
					["OutputFolder"] = outputFolder,
					["TableName"] = tableName ?? "null",
				});
		}
	}

	/// <summary>
	/// Creates a table in ClickHouse database using the generated schema.
	/// </summary>
	/// <param name="databaseDetails">Connection details for the ClickHouse database</param>
	/// <returns>True if the table was created successfully</returns>
	/// <exception cref="NotImplementedException">This method is not yet implemented</exception>
	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException("This feature is planned for future releases");
	}

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
		ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException("This feature is planned for future releases");
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	~ClickHouseClient()
	{
		this.Dispose();
	}
}
