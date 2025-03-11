using Newtonsoft.Json.Sqlify.DTOs;

namespace Newtonsoft.Json.Sqlify.Infrastructure.Interfaces;

public interface INewtonsoftJsonSqlifyConfigurator
{
	INewtonsoftJsonSqlifyConfigurator AddOptions(Option option);
}
