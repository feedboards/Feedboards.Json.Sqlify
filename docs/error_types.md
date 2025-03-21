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

### NestedStructureLimitException (JSON)

Thrown when JSON nesting exceeds the configured depth limit.

**Error Code:** `JSN_002`

**Use Cases:**

- JSON structure is deeper than allowed maximum depth
- Complex nested objects exceed depth limit

**Example:**

```csharp
try
{
    client.GenerateSQL("deep.json", "table_name", maxDepth: 3);
}
catch (NestedStructureLimitException ex) when (ex.ErrorCode == "JSN_002")
{
    // Access error details
    var maxDepth = ex.Metadata["MaxAllowedDepth"];
    var actualDepth = ex.Metadata["ActualDepth"];
}
```

### NestedStructureLimitException (SQL)

Thrown when SQL nesting exceeds the configured depth limit during SQL generation.

**Error Code:** `SQL_001`

**Use Cases:**

- Generated SQL structure is deeper than allowed maximum depth
- Complex nested fields exceed SQL nesting limit

**Example:**

```csharp
try
{
    client.GenerateSQL("complex.json", "table_name", maxDepth: 2);
}
catch (NestedStructureLimitException ex) when (ex.ErrorCode == "SQL_001")
{
    // Access error details
    var maxDepth = ex.Metadata["MaxAllowedDepth"];
    var actualDepth = ex.Metadata["ActualDepth"];
    var tableName = ex.Metadata["TableName"];
    var nestedField = ex.Metadata["NestedField"];
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
├── NestedStructureLimitException
│   ├── JSON Limit (JSN_002)
│   └── SQL Limit (SQL_001)
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
catch (NestedStructureLimitException ex) when (ex.ErrorCode == "SQL_001")
{
    // Handle SQL nesting limit
}
catch (NestedStructureLimitException ex) when (ex.ErrorCode == "JSN_002")
{
    // Handle JSON nesting limit
}
catch (InvalidJsonStructureException ex)
{
    // Handle JSON error
}
catch (FeedboardsJsonSqlifyException ex)
{
    // Handle any other custom exception
}
catch (Exception ex)
{
    // Handle unexpected errors
}
```

2. **Use metadata for detailed error handling:**

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

3. **Check inner exceptions for root cause:**

```csharp
catch (FeedboardsJsonSqlifyException ex)
{
    var rootCause = ex;
    while (rootCause.InnerException != null)
    {
        rootCause = rootCause.InnerException as FeedboardsJsonSqlifyException;
        if (rootCause == null) break;
    }
    // Handle root cause
}
```
