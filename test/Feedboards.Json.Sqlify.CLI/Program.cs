// See https://aka.ms/new-console-template for more information
using Feedboards.Json.Sqlify.Clients.ClickHousel;

Console.WriteLine("Hello, World!");

var client = new ClickHouseClient();

//client.GenerateSQL(
//	@"F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\adyawater.com_products_1.json",
//	@"F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\test.sql",
//	"test_1");

client.GenerateSQL(
	@"F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\JSON",
	@"F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SQL");