using Feedboards.Json.Sqlify;
using Feedboards.Json.Sqlify.Clients.ClickHouse;
using Feedboards.Json.Sqlify.ErrorSystem.Exceptions;
using Feedboards.Json.Sqlify.ErrorSystem;
using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using Feedboards.Json.Sqlify.JSON.ClickHouse;
using CustomFileNotFoundException = Feedboards.Json.Sqlify.ErrorSystem.Exceptions.FileNotFoundException;
using System.Text.Json;
using Feedboards.Json.Sqlify.Clients.ClickHousel;
using System.Text;


//var types = new Dictionary<string, string>();
//types["name"] = "String";
//types["tags"] = "String";
//types["id"] = "Int";
//types["update_at"] = "DateTime";

//StringBuilder sb = new StringBuilder();
//sb.AppendLine("{");

//// Option 1: Leave the comma on each line
//foreach (var kvp in types)
//{
//	sb.AppendLine($"   `{kvp.Key}` {kvp.Value},");
//}

//sb.AppendLine("}");
//Console.WriteLine(sb.ToString());

//List<string> allTypes = [
//	"Int",
//	"String",
//	"DataTime"
//];
//var formattedTypes = string.Empty;
//foreach (var type in allTypes)
//{
//	if (string.IsNullOrEmpty(formattedTypes))
//	{
//		formattedTypes = type.ToString().Trim();
//	}
//	else
//	{
//		formattedTypes += $", {type.ToString().Trim()}";
//	}
//}

//Console.WriteLine($"Tuple({formattedTypes})");


//var count = 10;
//for (var i = 1; i <= count / 2; i++)
//{
//	if (i == 1)
//	{
//		Console.WriteLine($"The first array: {i}");
//		Console.WriteLine($"The second array: {i + 1}");
//	}
//	else
//	{
//		Console.WriteLine($"The first array: {i + 1}");
//	}
//}

var client = new ClickHouseClient();

//client.GenerateSQLAndWrite(
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\MRF\\2025-03-01_7-ELEVEN-INC_index.json",
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\MRF\\2025-03-01_7-ELEVEN-INC_index.sql",
//	"eleven_inc_index");

client.GenerateSQLAndWrite(
	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\SimpleTest\\simple.json",
	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\SimpleTest\\simple.sql",
	"eleven_inc_index");

//Console.WriteLine("Testing error handling in ClickHouseClient...\n");

//// Create a sample JSON file for testing
//var sampleJson = @"{
//    ""id"": 1,
//    ""name"": ""Test Product"",
//    ""price"": 10.99,
//    ""details"": {
//        ""color"": ""red"",
//        ""size"": ""large"",
//        ""nested"": {
//            ""level1"": {
//                ""level2"": {
//                    ""level3"": {
//                        ""level4"": {
//                            ""level5"": {
//                                ""level6"": {
//                                    ""level7"": {
//                                        ""level8"": {
//                                            ""level9"": {
//                                                ""level10"": {
//                                                    ""level11"": ""too deep""
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}";
//File.WriteAllText("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", sampleJson);

//var options = new ClickHouseOption
//{
//    PathToFolderWithJson = "F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json",
//    PathToOutputFolder = "F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/SQL/"
//};

//var client = new ClickHouseClient(options);

//Console.WriteLine("Test 1: Invalid table name with special characters");
//try
//{
//    client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "test@table");
//}
//catch (InvalidTableNameException ex)
//{
//    Console.WriteLine($"✓ Caught InvalidTableNameException: {ex.Message}");
//    Console.WriteLine($"Table name: {ex.Metadata["TableName"]}");
//}

//Console.WriteLine("\nTest 2: Unlimited depth (should work with all levels)");
//try
//{
//    var sql = client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "products", 0);
//    Console.WriteLine($"✓ Successfully generated SQL with unlimited depth");
//    Console.WriteLine($"Generated SQL:\n{sql}");
//}
//catch (NestedStructureLimitException ex)
//{
//    Console.WriteLine($"✗ Failed: NestedStructureLimitException was thrown when it shouldn't have been");
//    Console.WriteLine($"Max allowed depth: {ex.Metadata["MaxAllowedDepth"]}");
//    Console.WriteLine($"Attempted depth: {ex.Metadata["ActualDepth"]}");
//}

//Console.WriteLine("\nTest 3: Invalid configuration - missing required option");
//try
//{
//    var invalidOptions = new ClickHouseOption(); // Empty options
//    var clientWithoutOptions = new ClickHouseClient(invalidOptions);
//}
//catch (InvalidConfigurationException ex)
//{
//    Console.WriteLine($"✓ Caught InvalidConfigurationException: {ex.Message}");
//    Console.WriteLine($"Missing configuration key: {ex.Metadata["ConfigKey"]}");
//}

//Console.WriteLine("\nTest 4: Invalid configuration - negative max depth (should work)");
//try
//{
//    var sql = client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/sample.json", "test_table", -1);
//    Console.WriteLine($"✓ Successfully generated SQL with negative depth (unlimited)");
//    Console.WriteLine($"Generated SQL:\n{sql}");
//}
//catch (InvalidConfigurationException ex)
//{
//    Console.WriteLine($"✗ Failed: InvalidConfigurationException was thrown when it shouldn't have been");
//    Console.WriteLine($"Configuration key: {ex.Metadata["ConfigKey"]}");
//}

//Console.WriteLine("\nTest 5: Invalid JSON structure");
//try
//{
//    var invalidJson = @"{
//        ""array"": [1, 2, 3,] // Invalid trailing comma
//    }";
//    File.WriteAllText("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json", invalidJson);
//    try
//    {
//        client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json", "test");
//    }
//    catch (InvalidJsonStructureException ex)
//    {
//        Console.WriteLine($"✓ Caught InvalidJsonStructureException: {ex.Message}");
//        Console.WriteLine($"JSON file path: {ex.Metadata["JsonPath"]}");
//        if (ex.InnerException != null)
//        {
//            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
//        }
//    }
//    finally
//    {
//        if (File.Exists("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json"))
//        {
//            File.Delete("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/invalid.json");
//        }
//    }
//}
//catch (CustomFileNotFoundException ex)
//{
//    Console.WriteLine($"✓ Caught FileNotFoundException: {ex.Message}");
//    Console.WriteLine($"File path: {ex.Metadata["FilePath"]}");
//    Console.WriteLine($"Reason: {ex.Metadata["Reason"]}");
//}

//Console.WriteLine("\nTest 6: File not found");
//try
//{
//    client.GenerateSQL("F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/nonexistent.json", "test");
//}
//catch (CustomFileNotFoundException ex)
//{
//    Console.WriteLine($"✓ Caught FileNotFoundException: {ex.Message}");
//    Console.WriteLine($"File path: {ex.Metadata["FilePath"]}");
//    Console.WriteLine($"Reason: {ex.Metadata["Reason"]}");
//}

//Console.WriteLine("\nAll error tests completed!");

//const string TEST_FOLDER = @"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test";

//// Test case: Nesting limit validation
//Console.WriteLine("\nTest case: Testing nesting limit validation");

//// Test 1: SQL nesting limit
//Console.WriteLine("\n1. Testing SQL nesting limit (should fail)");
//try
//{
//    var sqlOptions = new ClickHouseOption
//    {
//        PathToFolderWithJson = TEST_FOLDER
//    };

//    var sqlClient = new ClickHouseClient(sqlOptions);
//    var jsonContent = @"{
//        ""level1"": {
//            ""array"": [
//                {
//                    ""level2"": {
//                        ""array"": [
//                            {
//                                ""level3"": {
//                                    ""value"": 42
//                                }
//                            }
//                        ]
//                    }
//                }
//            ]
//        }
//    }";

//    var deepJsonPath = Path.Combine(TEST_FOLDER, "deep.json");
//    File.WriteAllText(deepJsonPath, jsonContent);
//    sqlClient.GenerateSQL(deepJsonPath, "test_table", 1); // Set maxDepth to 1
//    Console.WriteLine("✗ Failed: Expected NestedStructureLimitException was not thrown");
//}
//catch (NestedStructureLimitException ex)
//{
//    Console.WriteLine("✓ Caught expected NestedStructureLimitException");
//    Console.WriteLine($"Error Code: {ex.ErrorCode}");
//    Console.WriteLine($"Message: {ex.Message}");
//    Console.WriteLine("Metadata:");
//    foreach (var meta in ex.Metadata)
//    {
//        Console.WriteLine($"  {meta.Key}: {meta.Value}");
//    }
//}

//// Test 2: JSON nesting limit
//Console.WriteLine("\n2. Testing JSON nesting limit (should fail)");
//try
//{
//    var jsonContent = @"{
//        ""level1"": {
//            ""level2"": {
//                ""level3"": {
//                    ""level4"": {
//                        ""value"": 42
//                    }
//                }
//            }
//        }
//    }";

//    var deepJsonPath = Path.Combine(TEST_FOLDER, "deep_json.json");
//    File.WriteAllText(deepJsonPath, jsonContent);
//    var sqlOptions = new ClickHouseOption
//    {
//        PathToFolderWithJson = TEST_FOLDER
//    };

//    var sqlClient = new ClickHouseClient(sqlOptions);
//    sqlClient.GenerateSQL(deepJsonPath, "test_table", 2); // Set maxDepth to 2
//    Console.WriteLine("✗ Failed: Expected NestedStructureLimitException was not thrown");
//}
//catch (NestedStructureLimitException ex)
//{
//    Console.WriteLine("✓ Caught expected NestedStructureLimitException");
//    Console.WriteLine($"Error Code: {ex.ErrorCode}");
//    Console.WriteLine($"Message: {ex.Message}");
//    Console.WriteLine("Metadata:");
//    foreach (var meta in ex.Metadata)
//    {
//        Console.WriteLine($"  {meta.Key}: {meta.Value}");
//    }
//}

//// Test 3: Unlimited depth (should succeed)
//Console.WriteLine("\n3. Testing unlimited depth (should succeed)");
//try
//{
//    var sqlOptions = new ClickHouseOption
//    {
//        PathToFolderWithJson = TEST_FOLDER
//    };

//    var sqlClient = new ClickHouseClient(sqlOptions);
//    var deepJsonPath = Path.Combine(TEST_FOLDER, "deep.json");
//    var sql = sqlClient.GenerateSQL(deepJsonPath, "test_table", 0); // Set maxDepth to 0 for unlimited
//    Console.WriteLine("✓ Successfully generated SQL with unlimited depth");
//    Console.WriteLine($"Generated SQL:\n{sql}");
//}
//catch (NestedStructureLimitException ex)
//{
//    Console.WriteLine("✗ Failed: NestedStructureLimitException was thrown when it shouldn't have been");
//    Console.WriteLine($"Error Code: {ex.ErrorCode}");
//    Console.WriteLine($"Message: {ex.Message}");
//    Console.WriteLine("Metadata:");
//    foreach (var meta in ex.Metadata)
//    {
//        Console.WriteLine($"  {meta.Key}: {meta.Value}");
//    }
//}

//// Cleanup
//var deepJsonPath1 = Path.Combine(TEST_FOLDER, "deep.json");
//var deepJsonPath2 = Path.Combine(TEST_FOLDER, "deep_json.json");

//if (File.Exists(deepJsonPath1))
//{
//    File.Delete(deepJsonPath1);
//}
//if (File.Exists(deepJsonPath2))
//{
//    File.Delete(deepJsonPath2);
//} 