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
	/// Uses options from configuration.
	/// </summary>
	public bool GenerateSQL(string tableName, int? maxDepth = 10)
	{
		var jsonFolder = option?.PathToFolderWithJson
			?? throw new ArgumentException("Path to folder with JSON is not provided.");
		var outputFolder = option?.PathToOutputFolder
			?? throw new ArgumentException("Path to output folder is not provided.");

		return GenerateSQL(jsonFolder, outputFolder, tableName, maxDepth);
	}

	/// <summary>
	/// Uses the provided folder as specified by the folder type, and uses the configuration value for the other folder.
	/// </summary>
	public bool GenerateSQL(string folderPath, FolderType folderType, string? tableName = null, int? maxDepth = 10)
	{
		if (folderType == FolderType.JsonFolder)
		{
			var outputFolder = option?.PathToOutputFolder
				?? throw new ArgumentException("Path to output folder is not provided.");

			return GenerateSQL(folderPath, outputFolder, maxDepth: maxDepth);
		}
		else // FolderType.OutputFolder
		{
			var jsonFolder = option?.PathToFolderWithJson
				?? throw new ArgumentException("Path to folder with JSON is not provided.");

			return GenerateSQL(jsonFolder, folderPath, tableName, maxDepth);
		}
	}

	/// <summary>
	/// Uses both provided folder paths.
	/// </summary>
	public bool GenerateSQL(string jsonFolder, string outputFolder, string? tableName = null, int? maxDepth = 10)
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

				var jsonText = File.ReadAllText(jsonFolder, Encoding.UTF8);
				using var document = JsonDocument.Parse(jsonText);

				var jsonData = document.RootElement;

				var jsonAnalyzer = new ClickHouseJsonAnalyzer();

				var structure = jsonAnalyzer.AnalyzeJsonStructure(jsonData, "", (int)maxDepth, 0);

				var sqlBuilder = new ClickHouseSQLBuilder();

				var schema = sqlBuilder.GenerateClickHouseSchema(structure, tableName);

				string outputPath = Path.GetFullPath(outputFolder);
				File.WriteAllText(outputPath, schema);
			}
		}
		catch (Exception exc)
		{
			Console.Error.WriteLine(exc);
		}

		return true;
	}

	//TODO
	public bool CreateTable(ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException();
	}

	//TODO
	public void GenerateSQLAndCreateTable(string? pathToFolderWithJson = null, string? PathToOutputFolder = null, ClickHouseDatabaseDetails? databaseDetails = null)
	{
		throw new NotImplementedException();
	}
}
