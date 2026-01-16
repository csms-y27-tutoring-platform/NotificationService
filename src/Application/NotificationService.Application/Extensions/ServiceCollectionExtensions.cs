using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstractions.Messaging;
using NotificationService.Application.Contracts.Services;
using NotificationService.Application.Messaging;

namespace NotificationService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, Services.NotificationsService>();
        services.AddSingleton<INotificationEventProcessor, NotificationEventProcessor>();

        return services;
    }
}