using Microsoft.Extensions.DependencyInjection;
using Feedboards.Json.Sqlify.DTOs;
using Feedboards.Json.Sqlify.Infrastructure.Interfaces;

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

	public IFeedboardsJsonSqlifyConfigurator AddOptions(Option option)
	{
		// TODO Add services

		return this;
	}
}
