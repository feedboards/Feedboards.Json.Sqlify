# Exception Types

This document describes the exception types used in the Feedboards.Json.Sqlify library.

## Base Exception

### FeedboardsJsonSqlifyException

The base exception class for all custom exceptions in the library.

**Properties:**

- `ErrorCode`: A unique code identifying the error type
- `Message`: A human-readable error message
- `Metadata`: A dictionary containing additional error context
- `InnerException`: The original exception that caused this error (if any)

## Specific Exceptions

### FileNotFoundException

Thrown when a required file cannot be found.

**Error Code:** `FILE_001`

**Use Cases:**

- File path is invalid
- File does not exist
- Directory does not exist

**Example:**

```csharp
try
{
    client.GenerateSQL("nonexistent.json", "table_name");
}
catch (FileNotFoundException ex)
{
    // Access error details
    var filePath = ex.Metadata["FilePath"];
    var reason = ex.Metadata["Reason"];
}
```

### InvalidConfigurationException

Thrown when configuration options are invalid or missing.

**Error Code:** `CFG_001`

**Use Cases:**

- Required configuration option is missing
- Configuration value is invalid
- Configuration combination is invalid

**Example:**

```csharp
try
{
    var options = new ClickHouseOption(); // Empty options
    var client = new ClickHouseClient(options);
}
catch (InvalidConfigurationException ex)
{
    // Access error details
    var configKey = ex.Metadata["ConfigKey"];
}
```

### InvalidTableNameException

Thrown when a table name is invalid.

**Error Code:** `TBL_001`

**Use Cases:**

- Table name is null or empty
- Table name contains invalid characters
- Table name doesn't match required pattern

**Example:**

```csharp
try
{
    client.GenerateSQL("data.json", "invalid@table");
}
catch (InvalidTableNameException ex)
{
    // Access error details
    var tableName = ex.Metadata["TableName"];
}
```

### InvalidJsonStructureException

Thrown when JSON data is invalid or cannot be parsed.

**Error Code:** `JSN_001`

**Use Cases:**

- JSON syntax is invalid
- JSON structure doesn't match expected format
- JSON contains unsupported values

**Example:**

```csharp
try
{
    client.GenerateSQL("invalid.json", "table_name");
}
catch (InvalidJsonStructureException ex)
{
    // Access error details
    var jsonPath = ex.Metadata["JsonPath"];
    var innerError = ex.InnerException?.Message;
}
```

### DatabaseConnectionFailedException

Thrown when database connection fails.

**Error Code:** `DB_001`

**Use Cases:**

- Cannot connect to database
- Authentication failed
- Network issues

**Example:**

```csharp
try
{
    var details = new ClickHouseDatabaseDetails
    {
        Host = "invalid-host",
        Port = 8123,
        Database = "test_db"
    };
    client.CreateTable(details);
}
catch (DatabaseConnectionFailedException ex)
{
    // Access error details
    var host = ex.Metadata["Host"];
    var port = ex.Metadata["Port"];
    var database = ex.Metadata["Database"];
}
```

## Exception Hierarchy

```
FeedboardsJsonSqlifyException
├── FileNotFoundException
├── InvalidConfigurationException
├── InvalidTableNameException
├── InvalidJsonStructureException
└── DatabaseConnectionFailedException
```

## Best Practices

1. **Always catch specific exceptions first:**

```csharp
try
{
    // Your code
}
catch (InvalidTableNameException ex)
{
    // Handle table name error
}
catch (FeedboardsJsonSqlifyException ex)
{
    // Handle other custom exceptions
}
catch (Exception ex)
{
    // Handle unexpected exceptions
}
```

2. **Use the metadata to get detailed error information:**

```csharp
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

3. **Check the inner exception for more details:**

```csharp
catch (InvalidJsonStructureException ex)
{
    Console.WriteLine($"JSON Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Original Error: {ex.InnerException.Message}");
    }
}
```

4. **Use error codes for programmatic handling:**

```csharp
catch (FeedboardsJsonSqlifyException ex)
{
    switch (ex.ErrorCode)
    {
        case "FILE_001":
            // Handle file not found
            break;
        case "JSN_001":
            // Handle JSON structure error
            break;
        // Handle other error codes
    }
}
```
