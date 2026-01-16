using BookingService.Presentation.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Abstractions.Messaging;
using NotificationService.Presentation.Kafka.Options;

namespace NotificationService.Infrastructure.Kafka.Consumers;

public class NotificationEventConsumer : INotificationEventConsumer
{
    private readonly IConsumer<byte[], byte[]> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConnectionOptions _connectionOptions;
    private readonly ConsumerOptions _consumerOptions;
    private readonly IDeserializer<BookingEventKey> _keyDeserializer;
    private readonly IDeserializer<BookingEventValue> _valueDeserializer;
    private readonly ILogger<NotificationEventConsumer> _logger;
    private CancellationTokenSource? _cancellationTokenSource;

    public NotificationEventConsumer(
        IOptions<ConnectionOptions> connectionOptions,
        IOptions<ConsumerOptions> consumerOptions,
        IDeserializer<BookingEventKey> keyDeserializer,
        IDeserializer<BookingEventValue> valueDeserializer,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotificationEventConsumer> logger)
    {
        _connectionOptions = connectionOptions.Value;
        _consumerOptions = consumerOptions.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;

        var config = new ConsumerConfig
        {
            BootstrapServers = _connectionOptions.Host,
            SecurityProtocol = _connectionOptions.SecurityProtocol,
            GroupId = _consumerOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            MaxPollIntervalMs = 300000,
            SessionTimeoutMs = 10000,
        };

        _consumer = new ConsumerBuilder<byte[], byte[]>(config).Build();
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        string[] topics = new[]
        {
            _consumerOptions.BookingCreatedTopic,
            _consumerOptions.BookingCancelledTopic,
            _consumerOptions.BookingCompletedTopic,
        };

        _consumer.Subscribe(topics);

        Task.Run(
            async () =>
            {
                await ConsumeMessagesAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
            },
            _cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    public void StopConsuming()
    {
        _cancellationTokenSource?.Cancel();
        _consumer.Close();
    }

    private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<byte[], byte[]> consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult.Message.Value is null)
                    {
                        continue;
                    }

                    await ProcessMessageAsync(consumeResult.Message, cancellationToken).ConfigureAwait(false);
                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopped");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error in Kafka consumer");
            throw;
        }
    }

    private async Task ProcessMessageAsync(Message<byte[], byte[]> message, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        INotificationEventProcessor eventProcessor = scope.ServiceProvider.GetRequiredService<INotificationEventProcessor>();

        try
        {
            BookingEventKey key = _keyDeserializer.Deserialize(message.Key, false, SerializationContext.Empty);
            BookingEventValue value = _valueDeserializer.Deserialize(message.Value, false, SerializationContext.Empty);

            switch (value.EventCase)
            {
                case BookingEventValue.EventOneofCase.BookingCreated:
                    BookingCreated bookingCreated = value.BookingCreated;
                    await eventProcessor.ProcessBookingCreatedAsync(
                        bookingCreated.BookingId,
                        bookingCreated.CreatedBy,
                        cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Processed BookingCreated event for booking {BookingId}", bookingCreated.BookingId);
                    break;

                case BookingEventValue.EventOneofCase.BookingCancelled:
                    BookingCancelled bookingCancelled = value.BookingCancelled;
                    await eventProcessor.ProcessBookingCancelledAsync(
                        bookingCancelled.BookingId,
                        bookingCancelled.CancelledBy,
                        bookingCancelled.Reason,
                        cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Processed BookingCancelled event for booking {BookingId}", bookingCancelled.BookingId);
                    break;

                case BookingEventValue.EventOneofCase.BookingCompleted:
                    _logger.LogInformation("Received BookingCompleted event for booking {BookingId}", value.BookingCompleted.BookingId);
                    break;

                default:
                    _logger.LogWarning("Unknown event type received: {EventType}", value.EventCase);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Kafka message");
            throw;
        }
    }
}