using Azure.Storage.Blobs;
using FplBot.Options;
using FplBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FplBot
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            builder
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables();
                    // configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile($"appsettings.json", true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                    configApp.AddEnvironmentVariables();
                    // configApp.AddCommandLine(args);
                })
                .ConfigureWebJobs((context, b) =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddTimers();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.AddConsole();
                    // b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = context.Configuration.GetSection("ApplicationInsights:InstrumentationKey").Value);
                })
                .ConfigureServices((hostBuilderContext, s) =>
                {
                    // s.AddApplicationInsightsTelemetry(); // TODO: maybe at some point
                    s.AddScoped<IFplService, FplService>();
                    s.AddScoped<IOutputService, OutputService>();
                    s.AddScoped<IPersistenceService, PersistenceService>();
                    s.AddScoped<IEmailService, EmailService>();
                    s.AddScoped<IAttachmentService, AttachmentService>();
                    s.AddScoped<Logging.ILogger, Logging.Logger>();
                    s.AddScoped(s =>
                    {
                        var connectionString = hostBuilderContext.Configuration.GetSection("Azure:BlobConnectionString").Value;
                        var containerName = hostBuilderContext.Configuration.GetSection("Azure:ContainerName").Value;

                        var bsclient = new BlobServiceClient(connectionString);

                        return bsclient.GetBlobContainerClient(containerName);
                    });
                    s.Configure<FplOptions>(hostBuilderContext.Configuration.GetSection("FPL"));
                    s.Configure<EmailOptions>(hostBuilderContext.Configuration.GetSection("Email"));
                    s.Configure<AzureOptions>(hostBuilderContext.Configuration.GetSection("Azure"));
                });

            // builder.UseEnvironment(Environments.Development); // TODO: set this from appsettings?

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}

