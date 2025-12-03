using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Integrations.GoogleDrive.Options;

internal sealed class Options(IConfiguration configuration)
{
    public string? KeyVaultUri => configuration["KeyVaultUri"];

    public IConfigurationSection Backup => configuration.GetSection(BackupOptions.SectionName);

    public IConfigurationSection GoogleDrive => configuration.GetSection(GoogleDriveOptions.SectionName);
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
