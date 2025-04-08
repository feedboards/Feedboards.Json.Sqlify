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
