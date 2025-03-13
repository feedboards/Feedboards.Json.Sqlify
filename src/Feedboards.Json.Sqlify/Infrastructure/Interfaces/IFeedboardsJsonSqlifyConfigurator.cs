using Feedboards.Json.Sqlify.DTOs.ClickHouse;

namespace Feedboards.Json.Sqlify.Infrastructure.Interfaces;

public interface IFeedboardsJsonSqlifyConfigurator
{
	IFeedboardsJsonSqlifyConfigurator UseCLickHouseSchema(ClickHouseOption option);

	//TODO add these methods

	//IFeedboardsJsonSqlifyConfigurator UseMSSQLSchema();
	//IFeedboardsJsonSqlifyConfigurator UseMySQLSchema();
	//IFeedboardsJsonSqlifyConfigurator UsePostgresSchema();
}
