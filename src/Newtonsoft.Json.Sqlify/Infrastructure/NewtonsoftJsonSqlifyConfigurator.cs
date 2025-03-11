﻿using Microsoft.Extensions.DependencyInjection;
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

	public INewtonsoftJsonSqlifyConfigurator AddDatabaseDetails()
	{
		// TODO Add services

		return this;
	}
}
