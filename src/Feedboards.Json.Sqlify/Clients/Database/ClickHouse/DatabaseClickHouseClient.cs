using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using RestSharp.Authenticators;
using RestSharp;
using System.Net;

namespace Feedboards.Json.Sqlify.Clients.Database.ClickHouse;

internal class DatabaseClickHouseClient
{
    private readonly ClickHouseDatabaseDetails databaseDetails;

    public DatabaseClickHouseClient(ClickHouseDatabaseDetails databaseDetails)
    {
        this.databaseDetails = databaseDetails;
    }

	public bool CreateTableInClickHouse(string query, string tableName)
	{
		// Build the ClickHouse URL
		string clickhouseUrl = $"http://{databaseDetails.Host}:{databaseDetails.Port}/";
		Console.WriteLine($"ClickHouse URI: {clickhouseUrl}");

		// Create the RestSharp client and request
		var options = new RestClientOptions(clickhouseUrl)
		{
			Authenticator = new HttpBasicAuthenticator(databaseDetails.User, databaseDetails.Password)
		};

		// Create the RestClient with the specified options
		var client = new RestClient(options);

		// Create the RestRequest; pass an empty resource ("") if using the root URL
		var request = new RestRequest("", Method.Post);
		request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

		// Add the query as the request body (form data)
		request.AddParameter("text/plain", query, ParameterType.RequestBody);

		try
		{
			// Execute the request and capture the response
			var response = client.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				Console.WriteLine($"Table {tableName} created successfully in ClickHouse");

				return true;
			}
			else
			{
				Console.WriteLine($"Error creating table: {response.Content}");

				return false;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error connecting to ClickHouse: {ex.Message}");

			throw;
		}

	}
}
