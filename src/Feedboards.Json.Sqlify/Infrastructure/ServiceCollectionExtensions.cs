using Microsoft.Extensions.DependencyInjection;
using Feedboards.Json.Sqlify.Infrastructure.Interfaces;

namespace Feedboards.Json.Sqlify.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddFeedboardsJsonSqlify(
		this IServiceCollection services,
		Action<IFeedboardsJsonSqlifyConfigurator> configure)
	{
		var configurator = new FeedboardsJsonSqlifyConfigurator(services);

		configure(configurator);

		return configurator.Services;
	}
}
