using Mav.Healthcheck.Api.Setup;

const string vaultSecretsMountPath = "/mnt/secrets-store";

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration
builder.Host
    .ConfigureAppConfiguration((_, config) =>
{
config
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", false, false);

config
    .AddJsonFile("appsettings.Development.json", true, false);

config
    .AddEnvironmentVariables()
    .AddKeyPerFile(directoryPath: vaultSecretsMountPath, optional: true);
})
    .ConfigureLogging((hostingContext, logging) =>
{
logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
logging.AddEventSourceLogger();
});
#pragma warning restore ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration

builder.ConfigureServices();

var app = builder.Build();

app.ConfigureRequestPipeline();

app.Run();

public partial class Program { }
