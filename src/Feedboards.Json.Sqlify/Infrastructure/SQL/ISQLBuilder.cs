namespace Feedboards.Json.Sqlify.Infrastructure.SQL;

public interface ISQLBuilder
{
    string GenerateSchema(Dictionary<string, string> structure, string tableName);
}