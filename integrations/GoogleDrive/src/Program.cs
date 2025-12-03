using Integrations.GoogleDrive.Backups;
using Integrations.GoogleDrive.Options;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<List<BackupOptions>>(builder.Options.Backup);
builder.Services.Configure<List<GoogleDriveOptions>>(builder.Options.GoogleDrive);

builder.Services.AddAzureClients(azureBuilder =>
{
    var keyVaultUri = builder.Options.KeyVaultUri ?? throw new InvalidOperationException("'KeyVaultUri' not set");
    azureBuilder.AddSecretClient(new Uri(keyVaultUri));
});

builder.Services.AddScoped<BackupOptionsResolver>();
builder.Services.AddScoped<BackupHandler>();

builder.Build().Run();
