namespace NotificationService.Application.Abstractions.Messaging;

public interface INotificationEventConsumer
{
    public Task StartConsumingAsync(CancellationToken cancellationToken);

    public void StopConsuming();
}