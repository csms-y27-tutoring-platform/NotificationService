using Microsoft.AspNetCore.Server.Kestrel.Core;
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

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.ListenLocalhost(5004, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        WebApplication app = builder.Build();

        app.MapGrpcService<NotificationService.Presentation.Grpc.Services.NotificationGrpcService>();

        app.Run();
    }
}