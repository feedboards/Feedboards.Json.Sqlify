using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Sqlify.DTOs;
using Newtonsoft.Json.Sqlify.Infrastructure.Interfaces;

namespace Newtonsoft.Json.Sqlify.Infrastructure;

public class NewtonsoftJsonSqlifyConfigurator : INewtonsoftJsonSqlifyConfigurator
{
	private readonly IServiceCollection services;

	public IServiceCollection Services
	{
		get { return services; }
	}

	public NewtonsoftJsonSqlifyConfigurator(IServiceCollection services)
    {
		this.services = services;
    }

	public INewtonsoftJsonSqlifyConfigurator AddOptions(Option option)
	{
		// TODO Add services

		return this;
	}
}
