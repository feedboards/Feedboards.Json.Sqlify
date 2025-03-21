# ClickHouse Error Codes

This document describes the error codes and exceptions that can be thrown by the ClickHouse client.

## Error Code Format

Error codes follow the format `XXX_NNN` where:

- `XXX` is a three-letter code indicating the error category
- `NNN` is a three-digit number uniquely identifying the error

## Error Categories

- `FILE`: File-related errors
- `CFG`: Configuration errors
- `TBL`: Table-related errors
- `JSN`: JSON-related errors
- `DB`: Database-related errors
- `SQL`: SQL generation and structure errors
- `UNK`: Unknown/unexpected errors

## Error Codes

### FILE_001: File Not Found

Thrown when a specified file cannot be found at the given path.

**Metadata:**

- `FilePath`: The path to the file that was not found
- `Reason`: Additional context about why the file was not found

### CFG_001: Invalid Configuration

Thrown when configuration options are invalid or missing.

**Metadata:**

- `ConfigKey`: The configuration key that was invalid or missing

### TBL_001: Invalid Table Name

Thrown when a table name is invalid, empty, or contains invalid characters.

**Metadata:**

- `TableName`: The invalid table name that was provided

### JSN_001: Invalid JSON Structure

Thrown when the JSON file contains invalid JSON or cannot be parsed.

**Metadata:**

- `JsonPath`: The path to the JSON file that was invalid
- `InnerException`: Details about the JSON parsing error

### JSN_002: Nested Structure Limit

Thrown when the JSON structure exceeds the maximum allowed nesting depth.

**Metadata:**

- `MaxAllowedDepth`: The maximum depth that was configured
- `ActualDepth`: The actual depth that was encountered

### DB_001: Database Connection Failed

Thrown when a connection to the ClickHouse database cannot be established.

**Metadata:**

- `Host`: The database host
- `Port`: The database port
- `Database`: The database name
- `InnerException`: Details about the connection failure

### SQL_001: Nested Structure Limit

Thrown when the generated SQL structure exceeds the maximum allowed nesting depth.

**Metadata:**

- `MaxAllowedDepth`: The maximum allowed depth for SQL nesting
- `ActualDepth`: The actual depth in the generated SQL
- `TableName`: The name of the table being generated
- `NestedField`: The name of the field where the limit was exceeded

### UNK_001: Unknown Error

Thrown when an unexpected error occurs that doesn't fit into other categories.

**Metadata:**

- Various metadata depending on the context of the error

## Usage Example

```csharp
try
{
    client.GenerateSQL("path/to/json", "table_name");
}
catch (InvalidTableNameException ex)
{
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Table Name: {ex.Metadata["TableName"]}");
}
catch (FeedboardsJsonSqlifyException ex)
{
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
    foreach (var data in ex.Metadata)
    {
        Console.WriteLine($"{data.Key}: {data.Value}");
    }
}
```

## Depth Limit Configuration

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
// you can also use negative numbers
client.GenerateSQL("path/to/json", "table_name", maxDepth: 0);

// Default (10 levels)
client.GenerateSQL("path/to/json", "table_name");
```

## Error Handling Best Practices

1. Always check for specific exceptions first, then catch the base `FeedboardsJsonSqlifyException`
2. Log the error code along with the exception message
3. Include relevant metadata in error reports
4. Use error codes for automated error handling and reporting
5. Consider implementing retry logic for transient errors (e.g., database connection issues)

## Contributing

If you encounter an error that doesn't have a corresponding error code, please:

1. Open an issue in the GitHub repository
2. Include the error details and scenario
3. Suggest an appropriate error code category
