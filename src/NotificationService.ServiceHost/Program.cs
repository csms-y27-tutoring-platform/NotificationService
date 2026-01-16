using FluentMigrator.Runner;
using NotificationService.Application.Extensions;
using NotificationService.Infrastructure.Kafka.Extensions;
using NotificationService.Infrastructure.Persistence.Extensions;
using NotificationService.Presentation.Grpc.Extensions;

namespace NotificationService.ServiceHost;

internal class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddPersistenceInfrastructure(builder.Configuration);
        builder.Services.AddKafkaInfrastructure(builder.Configuration);
        builder.Services.AddGrpcPresentation();

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        WebApplication app = builder.Build();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            IMigrationRunner migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            migrationRunner.MigrateUp();
        }

        app.MapGrpcService<NotificationService.Presentation.Grpc.Services.NotificationGrpcService>();

        app.Run();
    }
}