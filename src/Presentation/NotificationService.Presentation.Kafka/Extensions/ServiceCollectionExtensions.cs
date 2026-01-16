using BookingService.Presentation.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Presentation.Kafka.Options;
using NotificationService.Presentation.Kafka.Serializer;

namespace NotificationService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ConnectionOptions>().Bind(configuration.GetSection("Infrastructure:Kafka:Connection"));
        services.AddOptions<ConsumerOptions>().Bind(configuration.GetSection("Infrastructure:Kafka:Consumer"));

        services.AddSingleton<KafkaSerializer<BookingEventKey>>();
        services.AddSingleton<KafkaSerializer<BookingEventValue>>();
        services.AddSingleton<ISerializer<BookingEventKey>>(provider => provider.GetRequiredService<KafkaSerializer<BookingEventKey>>());
        services.AddSingleton<IDeserializer<BookingEventKey>>(provider => provider.GetRequiredService<KafkaSerializer<BookingEventKey>>());
        services.AddSingleton<ISerializer<BookingEventValue>>(provider => provider.GetRequiredService<KafkaSerializer<BookingEventValue>>());
        services.AddSingleton<IDeserializer<BookingEventValue>>(provider => provider.GetRequiredService<KafkaSerializer<BookingEventValue>>());

        return services;
    }
}