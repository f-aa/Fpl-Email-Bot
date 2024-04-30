using Azure.Storage.Blobs;
using FplBot.Options;
using FplBot.Services;

namespace FplBot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<FplJob>();

            builder.Services.AddScoped<IFplService, FplService>();
            builder.Services.AddScoped<IOutputService, OutputService>();
            builder.Services.AddScoped<IPersistenceService, PersistenceService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<Logging.ILogger, Logging.Logger>();
            builder.Services.AddScoped(s =>
            {
                var connectionString = builder.Configuration.GetSection("Azure:BlobConnectionString").Value;
                var containerName = builder.Configuration.GetSection("Azure:ContainerName").Value;

                var bsclient = new BlobServiceClient(connectionString);

                return bsclient.GetBlobContainerClient(containerName);
            });

            builder.Services.Configure<FplOptions>(builder.Configuration.GetSection("Fpl"));
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
            builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
