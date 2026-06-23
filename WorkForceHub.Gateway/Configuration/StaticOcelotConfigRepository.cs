using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;

namespace WorkForceHub.Gateway.Configuration;

public class StaticOcelotConfigRepository : IFileConfigurationRepository
{
    private readonly FileConfiguration _config;

    public StaticOcelotConfigRepository(FileConfiguration config)
    {
        _config = config;
    }

    public Task<Response<FileConfiguration>> Get()
        => Task.FromResult<Response<FileConfiguration>>(new OkResponse<FileConfiguration>(_config));

    public Task<Response> Set(FileConfiguration fileConfiguration)
        => Task.FromResult<Response>(new OkResponse());
}
