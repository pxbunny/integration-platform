using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Integrations.Gmail.Options;

internal sealed class Options(IConfiguration configuration)
{
    public IConfigurationSection Smtp => configuration.GetSection(SmtpOptions.SectionName);
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
