using Microsoft.Extensions.DependencyInjection;
using NotificationService.Presentation.Grpc.Services;

namespace NotificationService.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcPresentation(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddScoped<NotificationGrpcService>();

        return services;
    }
}