// See https://aka.ms/new-console-template for more information
using Feedboards.Json.Sqlify.Clients.ClickHousel;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Hello, World!");

var client = new ClickHouseClient();

//client.GenerateSQL(
//	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\adyawater.com_products_1.json",
//	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\test_2.sql",
//	"test_2");

client.GenerateSQL(
	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SimpleTest\test.json",
	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SimpleTest\test.sql",
	"test");

//client.GenerateSQL(
//	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\JSON",
//	@"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SQL");


//var filePath = @"F:\Projects\src\Feedboards.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SimpleTest\test.json";
//var clickHouseUrl = "http://localhost:8123/?query=INSERT%20INTO%20my_first_table%20FORMAT%20JSONEachRow";

//// Set your ClickHouse credentials
//var username = "default";
//var password = "clickhouse";  // Replace with your actual password

//try
//{
//	using var client = new HttpClient();

//	// Add basic authentication header
//	var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
//	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

//	// Read JSON file content
//	var jsonContent = await File.ReadAllTextAsync(filePath);
//	var content = new StringContent(jsonContent);
//	content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

//	// Post JSON data to ClickHouse
//	var response = await client.PostAsync(clickHouseUrl, content);
//	var responseText = await response.Content.ReadAsStringAsync();

//	if (response.IsSuccessStatusCode)
//	{
//		Console.WriteLine("Data loaded successfully.");
//	}
//	else
//	{
//		Console.WriteLine($"Error loading data: {responseText}");
//	}
//}
//catch (Exception ex)
//{
//	Console.WriteLine($"Exception occurred: {ex.Message}");
//}