using Feedboards.Json.Sqlify.Clients.ClickHouse;
using Feedboards.Json.Sqlify.Clients.ClickHouse.Interfaces;
using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using Feedboards.Json.Sqlify.JSON.ClickHouse;
using Feedboards.Json.Sqlify.SQL.ClickHouse;
using System.Text;
using System.Text.Json;

namespace Feedboards.Json.Sqlify.Clients.ClickHousel;

public class ClickHouseClient : IClickHouseClient
{
	private readonly ClickHouseOption? option;

	public ClickHouseClient(ClickHouseOption? option = null)
	{
		this.option = option;
	}

	/// <summary>
	/// Generates SQL schema from JSON data and returns it as a string.
	/// Uses configuration options for input path.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="ArgumentException">Thrown when PathToFolderWithJson is not provided in options</exception>
	public string GenerateSQL(string tableName, int? maxDepth = 10)
	{
		var jsonFolder = option?.PathToFolderWithJson
			?? throw new ArgumentException("Path to folder with JSON is not provided.");

		return GenerateSQL(jsonFolder, tableName, maxDepth);
	}

	/// <summary>
	/// Generates SQL schema from JSON data and returns it as a string.
	/// Uses the provided JSON file path.
	/// </summary>
	/// <param name="jsonFolder">Path to the JSON file</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>Generated SQL schema as a string</returns>
	/// <exception cref="ArgumentException">Thrown when tableName is null or empty, or when maxDepth is null</exception>
	/// <exception cref="FileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="JsonException">Thrown when the JSON file contains invalid JSON</exception>
	public string GenerateSQL(string jsonFolder, string? tableName = null, int? maxDepth = 10)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			throw new ArgumentException("Invalid Argument: Argument tableName cannot be null");
		}

		if (maxDepth == null)
		{
			throw new ArgumentException("Invalid Argument: Argument maxDepth cannot be null");
		}

		var jsonText = File.ReadAllText(jsonFolder, Encoding.UTF8);
		using var document = JsonDocument.Parse(jsonText);

		var jsonData = document.RootElement;

		var jsonAnalyzer = new ClickHouseJsonAnalyzer();

		var structure = jsonAnalyzer.AnalyzeJsonStructure(jsonData, "", (int)maxDepth, 0);

		var sqlBuilder = new ClickHouseSQLBuilder();

		return sqlBuilder.GenerateClickHouseSchema(structure, tableName);
	}

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses configuration options for both input and output paths.
	/// </summary>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="ArgumentException">Thrown when PathToFolderWithJson or PathToOutputFolder is not provided in options</exception>
	public bool GenerateSQLAndWrite(string tableName, int? maxDepth = 10)
	{
		var jsonFolder = option?.PathToFolderWithJson
			?? throw new ArgumentException("Path to folder with JSON is not provided.");
		var outputFolder = option?.PathToOutputFolder
			?? throw new ArgumentException("Path to output folder is not provided.");

		return GenerateSQLAndWrite(jsonFolder, outputFolder, tableName, maxDepth);
	}

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses the provided folder path and configuration for the other folder.
	/// </summary>
	/// <param name="folderPath">Path to the folder containing JSON files or output folder</param>
	/// <param name="folderType">Type of the provided folder (JsonFolder or OutputFolder)</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="ArgumentException">Thrown when the required configuration option is not provided</exception>
	public bool GenerateSQLAndWrite(string folderPath, FolderType folderType, string? tableName = null, int? maxDepth = 10)
	{
		if (folderType == FolderType.JsonFolder)
		{
			var outputFolder = option?.PathToOutputFolder
				?? throw new ArgumentException("Path to output folder is not provided.");

			return GenerateSQLAndWrite(folderPath, outputFolder, maxDepth: maxDepth);
		}
		else // FolderType.OutputFolder
		{
			var jsonFolder = option?.PathToFolderWithJson
				?? throw new ArgumentException("Path to folder with JSON is not provided.");

			return GenerateSQLAndWrite(jsonFolder, folderPath, tableName, maxDepth);
		}
	}

	/// <summary>
	/// Generates SQL schema from JSON data and writes it to a file.
	/// Uses provided paths for both input and output.
	/// </summary>
	/// <param name="jsonFolder">Path to the JSON file or folder</param>
	/// <param name="outputFolder">Path to the output SQL file or folder</param>
	/// <param name="tableName">Name of the table to generate</param>
	/// <param name="maxDepth">Maximum depth for nested structures (default: 10)</param>
	/// <returns>True if the operation was successful</returns>
	/// <exception cref="ArgumentException">Thrown when paths are invalid or when tableName is null</exception>
	/// <exception cref="FileNotFoundException">Thrown when the JSON file does not exist</exception>
	/// <exception cref="JsonException">Thrown when the JSON file contains invalid JSON</exception>
	/// <exception cref="IOException">Thrown when there are issues writing to the output file</exception>
	public bool GenerateSQLAndWrite(string jsonFolder, string outputFolder, string? tableName = null, int? maxDepth = 10)
	{
		//Check if it is a file or folder wth bunch of files
		var jsonFolderType = Utils.CheckPath(jsonFolder);
		var outputFolderType = Utils.CheckPath(outputFolder);

		if (jsonFolderType != outputFolderType)
		{
			throw new ArgumentException(
				"Invalid Path Combination: Both arguments must be of the same type." +
				"Please provide either two folder paths or two file paths." +
				"Mixing a folder path with a file path is not allowed");
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
			else //In this case arguments jsonFolder and outputFolder will have name of file.
			{
				if (string.IsNullOrEmpty(tableName))
				{
					throw new ArgumentException("Invalid Argument: Argument tableName cannot be null");
				}

				if (maxDepth == null)
				{
					throw new ArgumentException("Invalid Argument: Argument maxDepth cannot be null");
				}

				var outputPath = Path.GetFullPath(outputFolder);
				File.WriteAllText(
					outputPath,
					GenerateSQL(
						jsonFolder,
						tableName, 
						maxDepth));
			}
		}
		catch (Exception exc)
		{
			Console.Error.WriteLine(exc);
		}

		return true;
	}

	/// <summary>
	/// Creates a table in ClickHouse database using the generated schema.
	/// </summary>
	/// <param name="databaseDetails">Connection details for the ClickHouse database</param>
	/// <returns>True if the table was created successfully</returns>
	/// <exception cref="NotImplementedException">This method is not yet implemented</exception>
	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Generates SQL schema from JSON data and creates a table in ClickHouse database.
	/// </summary>
	/// <param name="pathToFolderWithJson">Path to the JSON file or folder</param>
	/// <param name="PathToOutputFolder">Path to the output SQL file or folder</param>
	/// <param name="databaseDetails">Connection details for the ClickHouse database</param>
	/// <exception cref="NotImplementedException">This method is not yet implemented</exception>
	public void GenerateSQLAndCreateTable(string? pathToFolderWithJson = null, string? PathToOutputFolder = null, ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException();
	}
}
