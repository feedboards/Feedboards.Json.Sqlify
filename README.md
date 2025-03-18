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

### Building from Source

```bash
# Clone the repository
git clone https://github.com/yourusername/Feedboards.Json.Sqlify.git
cd Feedboards.Json.Sqlify

# Build the solution
dotnet build
```

### NuGet Package (Coming Soon)

```bash
dotnet add package Feedboards.Json.Sqlify
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

// Generate SQL from a single JSON file
client.GenerateSQL(
    jsonFolder: "path/to/input.json",
    outputFolder: "path/to/output.sql",
    tableName: "my_table",
    maxDepth: 10  // Optional, defaults to 10
);

// Or process an entire directory of JSON files
client.GenerateSQL(
    jsonFolder: "path/to/json/folder",
    outputFolder: "path/to/sql/folder"
);
// Note: When processing a directory, table names will be derived from JSON filenames.
// Avoid using special characters (like dots) in filenames.
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

> **Note**: Some methods shown in the API Reference are currently in development and may not work as expected. Only the methods demonstrated in the Quick Start section are fully implemented and tested.

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

### Configuration

```csharp
public class ClickHouseOption
{
    public string? PathToFolderWithJson { get; set; }
    public string? PathToOutputFolder { get; set; }
}
```

## Data Type Mapping

| JSON Type        | ClickHouse Type | Notes                               |
| ---------------- | --------------- | ----------------------------------- |
| Number (integer) | UInt64/Int64    | Automatically detects integer range |
| Number (float)   | Float64         | For decimal numbers                 |
| String           | String          | UTF-8 encoded                       |
| Boolean          | UInt8           | 0 for false, 1 for true             |
| Array            | Array or Nested | Based on content type               |
| Object           | Nested          | Creates nested structure            |
| null             | Nullable        | Makes the field nullable            |

## Best Practices

1. Always specify a table name when processing single files
2. Keep nested structures within reasonable depth (recommended max: 10)
3. Ensure consistent JSON structure across files when processing directories
4. Use appropriate file permissions for input/output directories
5. Avoid using special characters in filenames when processing directories
6. Consider the performance impact of deeply nested structures

## Error Handling

The library provides error handling for common scenarios:

- Invalid file paths
- Mismatched folder/file combinations
- Invalid JSON structures
- Excessive nesting depth
- Missing required parameters
- File permission issues
- Invalid table names

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. Before submitting:

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, please:

1. Check the [documentation](docs/)
2. Open an issue in the GitHub repository
3. Contact the maintainers

## Roadmap

- [ ] Add an advanced error system
- [ ] Add support for MSSQL
- [ ] Add support for PostgreSQL
- [ ] Add support for MySQL
- [ ] Improve type inference
- [ ] Add support for custom type mappings
- [ ] Add support for schema validation
