using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstractions.Messaging;
using NotificationService.Infrastructure.Kafka.Consumers;
using NotificationService.Presentation.Kafka.Extensions;

namespace NotificationService.Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaPresentation(configuration);

        services.AddSingleton<INotificationEventConsumer, NotificationEventConsumer>();
        services.AddHostedService<KafkaConsumerHostedService>();

        return services;
    }
}