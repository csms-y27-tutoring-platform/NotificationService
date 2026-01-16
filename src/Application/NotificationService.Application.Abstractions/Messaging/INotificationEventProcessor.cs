namespace NotificationService.Application.Abstractions.Messaging;

public interface INotificationEventProcessor
{
    public Task ProcessBookingCreatedAsync(string bookingId, string createdBy, CancellationToken cancellationToken);

    public Task ProcessBookingCancelledAsync(string bookingId, string cancelledBy, string reason, CancellationToken cancellationToken);
}