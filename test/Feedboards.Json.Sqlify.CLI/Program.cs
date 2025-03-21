using Feedboards.Json.Sqlify.Clients.ClickHousel;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;
using Feedboards.Json.Sqlify.ErrorSystem;
using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using CustomFileNotFoundException = Feedboards.Json.Sqlify.ErrorSystem.Exceptions.FileNotFoundException;

Console.WriteLine("Testing error handling in ClickHouseClient...\n");

// Create a sample JSON file for testing
var sampleJson = @"{
    ""id"": 1,
    ""name"": ""Test Product"",
    ""price"": 10.99,
    ""details"": {
        ""color"": ""red"",
        ""size"": ""large"",
        ""nested"": {
            ""level1"": {
                ""level2"": {
                    ""level3"": {
                        ""level4"": {
                            ""level5"": {
                                ""level6"": {
                                    ""level7"": {
                                        ""level8"": {
                                            ""level9"": {
                                                ""level10"": {
                                                    ""level11"": ""too deep""
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}";
File.WriteAllText("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", sampleJson);

var options = new ClickHouseOption
{
    PathToFolderWithJson = "F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json",
    PathToOutputFolder = "F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/SQL/"
};

var client = new ClickHouseClient(options);

Console.WriteLine("Test 1: Invalid table name with special characters");
try
{
    client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "test@table");
}
catch (InvalidTableNameException ex)
{
    Console.WriteLine($"✓ Caught InvalidTableNameException: {ex.Message}");
    Console.WriteLine($"Table name: {ex.Metadata["TableName"]}");
}

Console.WriteLine("\nTest 2: Unlimited depth (should work with all levels)");
try
{
    var sql = client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "products", 0);
    Console.WriteLine($"✓ Successfully generated SQL with unlimited depth");
    Console.WriteLine($"Generated SQL:\n{sql}");
}
catch (NestedStructureLimitException ex)
{
    Console.WriteLine($"✗ Failed: NestedStructureLimitException was thrown when it shouldn't have been");
    Console.WriteLine($"Max allowed depth: {ex.Metadata["MaxAllowedDepth"]}");
    Console.WriteLine($"Attempted depth: {ex.Metadata["ActualDepth"]}");
}

Console.WriteLine("\nTest 3: Invalid configuration - missing required option");
try
{
    var invalidOptions = new ClickHouseOption(); // Empty options
    var clientWithoutOptions = new ClickHouseClient(invalidOptions);
}
catch (InvalidConfigurationException ex)
{
    Console.WriteLine($"✓ Caught InvalidConfigurationException: {ex.Message}");
    Console.WriteLine($"Missing configuration key: {ex.Metadata["ConfigKey"]}");
}

Console.WriteLine("\nTest 4: Invalid configuration - negative max depth (should work)");
try
{
    var sql = client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "test_table", -1);
    Console.WriteLine($"✓ Successfully generated SQL with negative depth (unlimited)");
    Console.WriteLine($"Generated SQL:\n{sql}");
}
catch (InvalidConfigurationException ex)
{
    Console.WriteLine($"✗ Failed: InvalidConfigurationException was thrown when it shouldn't have been");
    Console.WriteLine($"Configuration key: {ex.Metadata["ConfigKey"]}");
}

Console.WriteLine("\nTest 5: Invalid JSON structure");
try
{
    var invalidJson = @"{
        ""array"": [1, 2, 3,] // Invalid trailing comma
    }";
    File.WriteAllText("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json", invalidJson);
    try
    {
        client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json", "test");
    }
    catch (InvalidJsonStructureException ex)
    {
        Console.WriteLine($"✓ Caught InvalidJsonStructureException: {ex.Message}");
        Console.WriteLine($"JSON file path: {ex.Metadata["JsonPath"]}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
    finally
    {
        if (File.Exists("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json"))
        {
            File.Delete("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json");
        }
    }
}
catch (CustomFileNotFoundException ex)
{
    Console.WriteLine($"✓ Caught FileNotFoundException: {ex.Message}");
    Console.WriteLine($"File path: {ex.Metadata["FilePath"]}");
    Console.WriteLine($"Reason: {ex.Metadata["Reason"]}");
}

Console.WriteLine("\nTest 6: File not found");
try
{
    client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/nonexistent.json", "test");
}
catch (CustomFileNotFoundException ex)
{
    Console.WriteLine($"✓ Caught FileNotFoundException: {ex.Message}");
    Console.WriteLine($"File path: {ex.Metadata["FilePath"]}");
    Console.WriteLine($"Reason: {ex.Metadata["Reason"]}");
}

Console.WriteLine("\nAll error tests completed!"); 