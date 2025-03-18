# Feedboards.Json.Sqlify

A .NET library for converting JSON data structures into SQL schemas and tables. Currently supports ClickHouse with planned support for MSSQL, PostgreSQL, MySQL, and more. This library helps you automatically generate SQL table definitions from JSON data, handling nested structures and complex data types.

## Features

- Automatic SQL schema generation from JSON files
- Support for nested JSON structures
- Handles both single files and directories of JSON files
- Configurable maximum depth for nested structures
- Automatic type detection and mapping to database-specific data types
- Support for ClickHouse Nested type structures
- Planned support for multiple databases:
  - ClickHouse (current)
  - MSSQL (planned)
  - PostgreSQL (planned)
  - MySQL (planned)

## Installation

```bash
dotnet add package Feedboards.Json.Sqlify  # Coming soon
```

## Quick Start

```csharp
// Initialize the client with options
var options = new ClickHouseOption
{
    PathToFolderWithJson = "path/to/json/files",
    PathToOutputFolder = "path/to/output"
};

var client = new ClickHouseClient(options);

// you can also do like this. In this case you need to provide all arguments in methods
var client = new ClickHouseClient(options);

// Generate SQL from a single JSON file
client.GenerateSQL(
    jsonFolder: "path/to/input.json",
    outputFolder: "path/to/output.sql",
    tableName: "my_table",
    // this is unnessary argument. iof you don't provide this, value will avtomaticly be 10
    maxDepth: 10
);

// Or process an entire directory of JSON files
client.GenerateSQL(
    jsonFolder: "path/to/json/folder",
    outputFolder: "path/to/sql/folder"
);
//In this case name of the table will be the same as the name of file.
//Please do not use any spetial charecter as . in the

```

## JSON to SQL Mapping Example

Input JSON:

```json
{
  "test": [
    {
      "name": "example",
      "title": "test1"
    }
  ]
}
```

Generated SQL:

```sql
CREATE TABLE IF NOT EXISTS test_table (
    `test` Nested(
        `name` String,
        `title` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();
```

## API Reference

### ClickHouseClient

The main class for generating SQL schemas from JSON.

#### Constructor

```csharp
public ClickHouseClient(ClickHouseOption? option = null)
```

#### Methods

##### GenerateSQL Overloads

1. Using configuration options:

```csharp
public bool GenerateSQL(string tableName, int? maxDepth = 10)
```

2. Using specific folder type:

```csharp
public bool GenerateSQL(string folderPath, FolderType folderType, string? tableName = null, int? maxDepth = 10)
```

3. Using explicit paths:

```csharp
public bool GenerateSQL(string jsonFolder, string outputFolder, string? tableName = null, int? maxDepth = 10)
```

Parameters:

- `jsonFolder`: Path to JSON file or folder
- `outputFolder`: Path where SQL files will be generated
- `tableName`: Name for the generated table
- `maxDepth`: Maximum nesting depth to process (default: 10)
- `folderType`: Specifies whether the path is for JSON input or SQL output

### Configuration

```csharp
public class ClickHouseOption
{
    public string? PathToFolderWithJson { get; set; }
    public string? PathToOutputFolder { get; set; }
}
```

## Data Type Mapping

| JSON Type        | ClickHouse Type |
| ---------------- | --------------- |
| Number (integer) | UInt64/Int64    |
| Number (float)   | Float64         |
| String           | String          |
| Boolean          | UInt8           |
| Array            | Array or Nested |
| Object           | Nested          |
| null             | Nullable        |

## Best Practices

1. Always specify a table name when processing single files
2. Keep nested structures within reasonable depth (recommended max: 10)
3. Ensure consistent JSON structure across files when processing directories
4. Use appropriate file permissions for input/output directories

## Error Handling

The library provides error handling for common scenarios:

- Invalid file paths
- Mismatched folder/file combinations
- Invalid JSON structures
- Excessive nesting depth
- Missing required parameters

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

[License Type] - see LICENSE file for details

## Support

For support, please open an issue in the GitHub repository.
