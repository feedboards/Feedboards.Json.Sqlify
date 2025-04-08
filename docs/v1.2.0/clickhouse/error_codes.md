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
