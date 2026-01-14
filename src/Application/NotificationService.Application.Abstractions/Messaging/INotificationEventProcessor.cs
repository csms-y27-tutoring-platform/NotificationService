namespace NotificationService.Application.Abstractions.Messaging;

public interface INotificationEventProcessor
{
    public Task ProcessBookingCreatedAsync(long bookingId, string createdBy, CancellationToken cancellationToken);

    public Task ProcessBookingCancelledAsync(long bookingId, string cancelledBy, string reason, CancellationToken cancellationToken);
}