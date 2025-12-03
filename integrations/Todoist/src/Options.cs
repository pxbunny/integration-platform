using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Integrations.Todoist;

internal sealed class Options(IConfiguration configuration)
{
    public string? TodoistApiBaseUrl => configuration["TodoistApiBaseUrl"];

    public string? TodoistApiKey => configuration["TodoistApiKey"];
}

internal static class OptionsExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public Options Options => GetInstance(builder.Configuration);
    }

    private static Options? _instance;

    private static Options GetInstance(IConfiguration configuration)
    {
        _instance ??= new Options(configuration);
        return _instance;
    }
}
