using Feedboards.Json.Sqlify.DTOs;

namespace Feedboards.Json.Sqlify.Infrastructure.Interfaces;

public interface IFeedboardsJsonSqlifyConfigurator
{
	IFeedboardsJsonSqlifyConfigurator AddOptions(Option option);
}
