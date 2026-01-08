using Integrations.Gmail.Options;
using Integrations.Gmail.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddOptions<SmtpOptions>().Bind(builder.Options.Smtp);
builder.Services.AddScoped<EmailSenderService>();

builder.Build().Run();
