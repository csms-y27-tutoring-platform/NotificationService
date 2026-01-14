using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Abstractions.Messaging;

namespace NotificationService.Infrastructure.Kafka;

public class KafkaConsumerHostedService : IHostedService
{
    private readonly INotificationEventConsumer _consumer;
    private readonly ILogger<KafkaConsumerHostedService> _logger;

    public KafkaConsumerHostedService(
        INotificationEventConsumer consumer,
        ILogger<KafkaConsumerHostedService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Kafka consumer");
        return _consumer.StartConsumingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Kafka consumer");
        _consumer.StopConsuming();
        return Task.CompletedTask;
    }
}