using Feedboards.Json.Sqlify.DTOs.ClickHouse;
using Feedboards.Json.Sqlify.Infrastructure.Interfaces;
using Feedboards.Json.Sqlify.SQL.ClickHouse;
using Microsoft.Extensions.DependencyInjection;

namespace Feedboards.Json.Sqlify.Infrastructure;

public class FeedboardsJsonSqlifyConfigurator : IFeedboardsJsonSqlifyConfigurator
{
	private readonly IServiceCollection services;

	public IServiceCollection Services
	{
		get { return services; }
	}

	public FeedboardsJsonSqlifyConfigurator(IServiceCollection services)
	{
		this.services = services;
	}

	public IFeedboardsJsonSqlifyConfigurator UseCLickHouseSchema(ClickHouseOption option)
	{
		services.AddSingleton<ClickHouseSQLBuilder>();

		return this;
	}
}
