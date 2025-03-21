# Feedboards.Json.Sqlify

A .NET library for converting JSON data structures into SQL schemas and tables. Currently supports ClickHouse with planned support for MSSQL, PostgreSQL, MySQL, and more. This library helps you automatically generate SQL table definitions from JSON data, handling nested structures and complex data types.

## Documentation

- [Error Codes](docs/clickhouse/errors_code.md) - Detailed list of error codes and their meanings
- [Exception Types](docs/error_types.md) - Exception types, usage, and best practices for error handling

## Features

- Automatic SQL schema generation from JSON files
- Support for nested JSON structures
- Handles both single files and directories of JSON files
- Configurable maximum depth for nested structures (including unlimited depth)
- Automatic type detection and mapping to database-specific data types
- Support for ClickHouse Nested type structures
- Comprehensive error handling system with detailed error codes
- Planned support for multiple databases:
  - ClickHouse (current)
  - MSSQL (planned)
  - PostgreSQL (planned)
  - MySQL (planned)

## Installation

### Building from Source

```bash
# Clone the repository
git clone https://github.com/feedboards/Feedboards.Json.Sqlify.git
cd Feedboards.Json.Sqlify

# Build the solution
dotnet build
```

### NuGet Package (Coming Soon)

```bash
dotnet add package Feedboards.Json.Sqlify
```

### Dependency Injection Setup

The library supports dependency injection in ASP.NET Core applications. Add the following code to your `Program.cs` or `Startup.cs`:

```csharp
using Feedboards.Json.Sqlify.Infrastructure;
using Feedboards.Json.Sqlify.DTOs.ClickHouse;

// In your ConfigureServices method or Program.cs
builder.Services.AddFeedboardsJsonSqlify(options =>
{
    // Configure ClickHouse options
    options.UseCLickHouseSchema(new ClickHouseOption
    {
        // All properties are optional and default to null
        PathToFolderWithJson = "path/to/json/files",  // Optional
        PathToOutputFolder = "path/to/output",        // Optional

        // Database details are optional and for future features
        DatabaseDetails = new ClickHouseDatabaseDetails
        {
            Host = "localhost",     // Required if DatabaseDetails is provided
            Port = 8123,            // Required if DatabaseDetails is provided
            User = "default",       // Required if DatabaseDetails is provided
            Password = "",          // Required if DatabaseDetails is provided
            Database = "default"    // Required if DatabaseDetails is provided
        }
    });

    // Or use minimal configuration
    options.UseCLickHouseSchema(); // All options are optional
});
```

Then you can inject and use the client in your services:

```csharp
public class YourService
{
    private readonly IClickHouseClient _clickHouseClient;

    public YourService(IClickHouseClient clickHouseClient)
    {
        _clickHouseClient = clickHouseClient;
    }

    public void ProcessJsonFile(string tableName)
    {
        try
        {
            // Using direct file paths (no configuration needed)
            var sqlSchema = _clickHouseClient.GenerateSQL(
                jsonFolder: "path/to/input.json",
                tableName: tableName
            );

            // Or using configured paths
            _clickHouseClient.GenerateSQLAndWrite(tableName);
        }
        catch (InvalidTableNameException ex)
        {
            // Handle invalid table name
            Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
            Console.WriteLine($"Invalid table name: {ex.Metadata["TableName"]}");
        }
        catch (FeedboardsJsonSqlifyException ex)
        {
            // Handle any other custom exception
            Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
            foreach (var data in ex.Metadata)
            {
                Console.WriteLine($"{data.Key}: {data.Value}");
            }
        }
    }
}
```

### Configuration Options

The `ClickHouseOption` class supports the following configuration:

```csharp
public class ClickHouseOption
{
    // Optional: Path to the folder containing JSON files
    // Defaults to null if not provided
    // Required only when using methods that read from files
    public string? PathToFolderWithJson { get; set; } = null;

    // Optional: Path where SQL files will be generated
    // Defaults to null if not provided
    // Required only when using methods that write to files
    public string? PathToOutputFolder { get; set; } = null;

    // Optional: Connection details for direct database operations
    // Defaults to null if not provided
    // Required only for future database integration features
    public ClickHouseDatabaseDetails? DatabaseDetails { get; set; } = null;
}

// Database connection details class
// All properties are required if DatabaseDetails is provided
public class ClickHouseDatabaseDetails
{
    public required string Host { get; set; }
    public required short Port { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
}
```

#### When to Provide Options

1. For generating SQL schema as string:

   - Provide `PathToFolderWithJson` when using configuration-based methods
   - Or use direct file path methods which don't require configuration

2. For writing SQL to files:

   - Provide both `PathToFolderWithJson` and `PathToOutputFolder` when using configuration-based methods
   - Or use direct file path methods which don't require configuration

3. For database operations (planned feature):
   - `DatabaseDetails` will be required with all its properties set

#### Example Usage

Without configuration (using direct paths):

```csharp
var client = new ClickHouseClient(); // No options needed
client.GenerateSQL(
    jsonFolder: "path/to/input.json",
    tableName: "my_table"
);
```

With configuration:

```csharp
var options = new ClickHouseOption
{
    PathToFolderWithJson = "path/to/json/files",
    PathToOutputFolder = "path/to/output"
};

var client = new ClickHouseClient(options);
client.GenerateSQL(tableName: "my_table");
```

With database details (for future features):

```csharp
var options = new ClickHouseOption
{
    DatabaseDetails = new ClickHouseDatabaseDetails
    {
        Host = "localhost",
        Port = 8123,
        User = "default",
        Password = "",
        Database = "default"
    }
};

var client = new ClickHouseClient(options);
```

## Quick Start

```csharp
try
{
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
        maxDepth: 10  // Optional, defaults to 10, use 0 for unlimited
    );

    // Or process an entire directory of JSON files
    client.GenerateSQL(
        jsonFolder: "path/to/json/folder",
        outputFolder: "path/to/sql/folder"
    );
    // Note: When processing a directory, table names will be derived from JSON filenames.
    // Avoid using special characters (like dots) in filenames.
}
catch (FeedboardsJsonSqlifyException ex)
{
    Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
    foreach (var data in ex.Metadata)
    {
        Console.WriteLine($"{data.Key}: {data.Value}");
    }
}
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

### Nested Structures and flatten_nested Setting

When dealing with nested structures inside other nested structures, the library automatically adds the `flatten_nested=0` setting to ensure proper handling of complex nested data.

### Depth Limit Configuration

The `maxDepth` parameter in `GenerateSQL` methods controls how deep the nested structures can go:

- Positive number (e.g., `maxDepth: 5`): Limits nesting to that specific depth
- Zero (`maxDepth: 0`): Unlimited depth
- Negative number (e.g., `maxDepth: -1`): Unlimited depth
- Default (no maxDepth specified): Limits to 10 levels

Example:

```csharp
// Limit to 5 levels
client.GenerateSQL("path/to/json", "table_name", maxDepth: 5);

// Unlimited nesting
client.GenerateSQL("path/to/json", "table_name", maxDepth: 0);

// Default (10 levels)
client.GenerateSQL("path/to/json", "table_name");
```

## Error Handling

The library provides a comprehensive error handling system with detailed error codes and metadata. For detailed information about error handling, see:

- [Error Codes Documentation](docs/clickhouse/errors_code.md)
- [Exception Types Documentation](docs/error_types.md)

Example of error handling:

```csharp
try
{
    client.GenerateSQL("path/to/json", "invalid@table");
}
catch (InvalidTableNameException ex)
{
    Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
    Console.WriteLine($"Invalid table name: {ex.Metadata["TableName"]}");
}
catch (InvalidJsonStructureException ex)
{
    Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
    Console.WriteLine($"JSON file: {ex.Metadata["JsonPath"]}");
    Console.WriteLine($"Error details: {ex.InnerException?.Message}");
}
catch (FeedboardsJsonSqlifyException ex)
{
    Console.WriteLine($"Error {ex.ErrorCode}: {ex.Message}");
    foreach (var data in ex.Metadata)
    {
        Console.WriteLine($"{data.Key}: {data.Value}");
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

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
