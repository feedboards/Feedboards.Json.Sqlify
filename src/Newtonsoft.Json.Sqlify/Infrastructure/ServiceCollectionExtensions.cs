using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Sqlify.Infrastructure.Interfaces;

namespace Newtonsoft.Json.Sqlify.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddNewtonsoftJsonSqlify(
		this IServiceCollection services,
		Action<INewtonsoftJsonSqlifyConfigurator> configure)
	{
		var configurator = new NewtonsoftJsonSqlifyConfigurator(services);

		configure(configurator);

		return configurator.Services;
	}
}
