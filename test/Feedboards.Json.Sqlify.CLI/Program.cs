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
using System.Text.RegularExpressions;


using (FileStream fs = File.OpenRead("F:\\Synthix\\data\\MRF\\index\\2025-03-01_715-Diesel-LLC-_index.json"))
{
	JsonElement jsonElement = ReadJsonElementFromStream(fs);
	Console.WriteLine(jsonElement.ToString());
}

JsonElement ReadJsonElementFromStream(Stream stream)
{
	// Parse the stream into a JsonDocument, and then clone the RootElement
	using (JsonDocument doc = JsonDocument.Parse(stream))
	{
		return doc.RootElement.Clone();
	}
}

var i = 0;

//var data = "Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String)";
//var data = "Nested(`reporting_plan_id` Nullable(String),`reporting_plan_id_type` Nullable(String),`ids` Nullable(Array(Int8)),`reporting_plans` Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String),`in_network_files` Nullable(Array(String)),`allowed_amount_file` Nullable(Nested(`description` String,`location` String)))";
//var t = data.Substring(7);
//var input = t.Remove(t.Length - 1);

//List<string> parts = SplitOnTopLevelCommas(input);

//// Now, process each part into key and value.
//Dictionary<string, string> dict = new Dictionary<string, string>();
//string pattern = @"^\s*(`[^`]+`)\s+(.+?)\s*$"; // group1: key, group2: value

//foreach (string part in parts)
//{
//	string trimmedPart = part.Trim();
//	Match match = Regex.Match(trimmedPart, pattern);
//	if (match.Success)
//	{
//		string key = match.Groups[1].Value;
//		string value = match.Groups[2].Value;
//		dict[key] = value;
//	}
//}

//// Print the dictionary to the console.
//foreach (var kv in dict)
//{
//	Console.WriteLine($"key - {kv.Key}");
//	Console.WriteLine($"value - {kv.Value}");
//	Console.WriteLine();
//}

//static List<string> SplitOnTopLevelCommas(string s)
//{
//	var result = new List<string>();
//	StringBuilder sb = new StringBuilder();
//	int parenCount = 0;

//	foreach (char c in s)
//	{
//		if (c == ',' && parenCount == 0)
//		{
//			result.Add(sb.ToString());
//			sb.Clear();
//		}
//		else
//		{
//			if (c == '(')
//				parenCount++;
//			else if (c == ')')
//				parenCount--;

//			sb.Append(c);
//		}
//	}

//	if (sb.Length > 0)
//		result.Add(sb.ToString());

//	return result;
//}

//var firstElement = new Dictionary<string, string>
//		{
//			{ "`reporting_structure`", "Nested(`reporting_plan_id` Nullable(String),`reporting_plan_id_type` Nullable(String),`ids` Nullable(Array(Int8)),`reporting_plans` Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String),`in_network_files` Nullable(Array(String)),`allowed_amount_file` Nullable(Nested(`description` String,`location` String)))" }
//		};

//// Suppose the second element is missing some properties inside the nested object.
//// For example, it might only have reporting_plan_id and reporting_plans.
//var secondElement = new Dictionary<string, string>
//		{
//			{ "`reporting_structure`", "Nested(`reporting_plan_id` Nullable(String),`reporting_plans` Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String))" }
//		};

//var result = SetNullable(firstElement, secondElement);

//foreach (var prop in result)
//{
//	Console.WriteLine($"{prop.Key} {prop.Value}");
//}

//static Dictionary<string, string> SetNullable(
//		Dictionary<string, string> firstElement,
//		Dictionary<string, string> secondElement)
//{
//	var setNullResult = new Dictionary<string, string>();

//	foreach (var property in firstElement)
//	{
//		// Check if the type is a Nested type (or a Nullable(Nested(...)))
//		if (property.Value.Contains("Nested("))
//		{
//			// Parse the nested definition from firstElement.
//			var firstNested = ParseNestedProperties(property.Value);

//			// Try to get the corresponding nested definition from secondElement.
//			Dictionary<string, string> secondNested = new Dictionary<string, string>();
//			if (secondElement.ContainsKey(property.Key) && secondElement[property.Key].Contains("Nested("))
//			{
//				secondNested = ParseNestedProperties(secondElement[property.Key]);
//			}

//			// Recursively set nullability on the nested level.
//			var updatedNested = SetNullable(firstNested, secondNested);

//			// Rebuild the Nested string.
//			// Preserve the Nullable wrapper if the second element doesn't contain the property.
//			string rebuilt = RebuildNestedString(updatedNested);
//			if (!secondElement.ContainsKey(property.Key))
//			{
//				setNullResult.Add(property.Key, $"Nullable(Nested({rebuilt}))");
//			}
//			else
//			{
//				setNullResult.Add(property.Key, $"Nested({rebuilt})");
//			}
//		}
//		else if (!secondElement.ContainsKey(property.Key))
//		{
//			// If the property is missing in secondElement, wrap it with Nullable(...).
//			setNullResult.Add(property.Key, $"Nullable({property.Value})");
//		}
//		else
//		{
//			setNullResult.Add(property.Key, property.Value);
//		}
//	}

//	return setNullResult;
//}

///// <summary>
///// Extracts inner properties from a Nested (or Nullable(Nested(...))) type string.
///// Returns a dictionary where the key is the property name (including its backticks)
///// and the value is its type definition.
///// </summary>
//static Dictionary<string, string> ParseNestedProperties(string nested)
//{
//	// Remove the Nullable wrapper if present.
//	if (nested.StartsWith("Nullable("))
//	{
//		// Assumes well-formed input.
//		nested = nested.Substring("Nullable(".Length, nested.Length - "Nullable(".Length - 1);
//	}
//	// Now nested should start with "Nested("
//	int start = nested.IndexOf("Nested(");
//	if (start >= 0)
//	{
//		nested = nested.Substring(start + "Nested(".Length, nested.Length - start - "Nested(".Length - 1);
//	}
//	// Now, nested contains the inner content, e.g.:
//	//   "`reporting_plan_id` Nullable(String),`reporting_plan_id_type` Nullable(String),..."
//	var entries = SplitOnTopLevelCommas(nested);
//	var dict = new Dictionary<string, string>();

//	// Regex to capture property name (in backticks) and the rest as its type.
//	string pattern = @"^\s*(`[^`]+`)\s+(.+?)\s*$";
//	foreach (var entry in entries)
//	{
//		Match m = Regex.Match(entry, pattern);
//		if (m.Success)
//		{
//			dict[m.Groups[1].Value] = m.Groups[2].Value;
//		}
//	}
//	return dict;
//}

///// <summary>
///// Rebuilds a Nested type string from a dictionary of property definitions.
///// The result will be of the form: "`prop1` type1,`prop2` type2,..."
///// </summary>
//static string RebuildNestedString(Dictionary<string, string> nestedDict)
//{
//	var parts = new List<string>();
//	foreach (var kvp in nestedDict)
//	{
//		parts.Add($"{kvp.Key} {kvp.Value}");
//	}
//	return string.Join(",", parts);
//}

///// <summary>
///// Splits a string on commas that are at the top level (i.e. not nested inside any parentheses).
///// </summary>
//static List<string> SplitOnTopLevelCommas(string s)
//{
//	var result = new List<string>();
//	StringBuilder sb = new StringBuilder();
//	int parenCount = 0;
//	foreach (char c in s)
//	{
//		if (c == ',' && parenCount == 0)
//		{
//			result.Add(sb.ToString());
//			sb.Clear();
//		}
//		else
//		{
//			if (c == '(')
//				parenCount++;
//			else if (c == ')')
//				parenCount--;
//			sb.Append(c);
//		}
//	}
//	if (sb.Length > 0)
//		result.Add(sb.ToString());
//	return result;
//}

//string input = "Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String)";

//string input = "`reporting_structure` Nested(`reporting_plan_id` Nullable(String),`reporting_plan_id_type` Nullable(String),`ids` Nullable(Array(Int8)),`reporting_plans` Nested(`plan_name` String,`plan_id` String,`plan_id_type` String,`plan_market_type` String),`in_network_files` Nullable(Array(String)),`allowed_amount_file` Nullable(Nested(`description` String,`location` String)))";

//// Pattern explanation:
////   `([^`]+)` matches the property name (inside backticks)
////   \s+ matches one or more whitespace characters
////   ([A-Za-z0-9_]+) captures the type (e.g., String)
//string pattern = @"`([^`]+)`\s+([A-Za-z0-9_]+)";

//// Find all matches in the input string
//MatchCollection matches = Regex.Matches(input, pattern);

//var elements = new Dictionary<string, string>();

//foreach (Match match in matches)
//{
//	elements[match.Groups[1].Value] = match.Groups[2].Value;
//}

//foreach (var prop in elements)
//{
//	Console.WriteLine($"{prop.Key} {prop.Value}");
//}

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
//	"F:\\Synthix\\data\\MRF\\index\\2025-03-01_715-Diesel-LLC-_index.json",
//	"F:\\Synthix\\data\\MRF\\SQL\\index\\2025-03-01_715-Diesel-LLC-_index.sql",
//	"mrf_index");

client.GenerateSQLAndWrite(
	"F:\\Synthix\\data\\MRF\\index",
	"F:\\Synthix\\data\\MRF\\SQL\\index");

//client.GenerateSQLAndWrite(
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\adyawater.com_products_1.json",
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\adyawater.com_products_1.sql",
//	"eleven_inc_index");

//client.GenerateSQLAndWrite(
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\MRF\\2025-03-01_7-ELEVEN-INC_index.json",
//	"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\MRF\\2025-03-01_7-ELEVEN-INC_index.sql",
//	"eleven_inc_index");

//client.GenerateSQLAndWrite(
//"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\SimpleTest\\simple.json",
//"F:\\Projects\\src\\Feedboards.Json.Sqlify\\test\\Feedboards.Json.Sqlify.CLI\\test\\SimpleTest\\simple.sql",
//"eleven_inc_index");

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